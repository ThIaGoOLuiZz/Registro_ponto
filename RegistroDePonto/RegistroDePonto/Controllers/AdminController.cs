using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistroDePonto.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace RegistroDePonto.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult CadastrarFuncionario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarFuncionario(Funcionario funcionario)
        {
            funcionario.RegistrosPonto = new List<RegistroPonto>();

            if (ModelState.IsValid)
            {
                var funcionarioExistente = _context.Funcionarios
                    .FirstOrDefault(f => f.Matricula == funcionario.Matricula);

                if (funcionarioExistente != null)
                {
                    ModelState.AddModelError("Matricula", "Matrícula já cadastrada.");
                    return View(funcionario);
                }

                _context.Funcionarios.Add(funcionario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(funcionario);
        }

        public IActionResult Index()
        {
            var funcionarios = _context.Funcionarios.ToList();
            return View(funcionarios);
        }

        [HttpPost]
        public IActionResult Logoff()
        {
            HttpContext.Session.Remove("Matricula");
            return RedirectToAction("Index", "Auth");
        }

        [HttpGet]
        public IActionResult GerarRelatorioPDF()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var funcionarios = _context.Funcionarios
                .Include(f => f.RegistrosPonto)
                .ToList();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4.Landscape());
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Text($"Relatório de Ponto - Todos os Dias")
                        .SemiBold().FontSize(14).FontColor(Colors.Blue.Darken2);

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(90);
                                columns.ConstantColumn(90);
                                columns.ConstantColumn(90);
                                columns.ConstantColumn(90);
                            });

                            table.Header(header =>
                            {
                                string[] headers = { "Matrícula", "Nome", "Data", "Entrada Manhã", "Saída Manhã", "Entrada Tarde", "Saída Tarde" };
                                foreach (var col in headers)
                                {
                                    header.Cell().Element(CellStyle).Text(col);
                                }

                                static IContainer CellStyle(IContainer container) =>
                                    container.DefaultTextStyle(x => x.SemiBold())
                                        .Padding(5)
                                        .Background(Colors.Grey.Lighten2)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Black);
                            });

                            foreach (var f in funcionarios)
                            {
                                var datas = f.RegistrosPonto
                                    .Select(r => r.DataRegistro)
                                    .Distinct()
                                    .OrderBy(d => d);

                                foreach (var data in datas)
                                {
                                    var batidas = f.RegistrosPonto.Where(r => r.DataRegistro == data).ToList();

                                    string entradaManha = batidas.FirstOrDefault(r => r.TipoBatida == "Entrada Manhã")?.HoraBatida.ToString(@"hh\:mm") ?? "PENDENTE";
                                    string saidaManha = batidas.FirstOrDefault(r => r.TipoBatida == "Saída Manhã")?.HoraBatida.ToString(@"hh\:mm") ?? "PENDENTE";
                                    string entradaTarde = batidas.FirstOrDefault(r => r.TipoBatida == "Entrada Tarde")?.HoraBatida.ToString(@"hh\:mm") ?? "PENDENTE";
                                    string saidaTarde = batidas.FirstOrDefault(r => r.TipoBatida == "Saída Tarde")?.HoraBatida.ToString(@"hh\:mm") ?? "PENDENTE";

                                    table.Cell().Padding(5).Text(f.Matricula.ToString());
                                    table.Cell().Padding(5).Text(f.NomeCompleto);
                                    table.Cell().Padding(5).Text(data.ToString("dd/MM/yyyy"));
                                    table.Cell().Padding(5).Text(entradaManha);
                                    table.Cell().Padding(5).Text(saidaManha);
                                    table.Cell().Padding(5).Text(entradaTarde);
                                    table.Cell().Padding(5).Text(saidaTarde);
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("Relatório gerado em ");
                            txt.Span($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}").SemiBold();
                        });
                });
            });

            var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"Relatorio_Ponto_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
