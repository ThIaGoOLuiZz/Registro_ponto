using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RegistroDePonto.Models;
using System;
using System.Linq;

namespace RegistroDePonto.Controllers
{
    public class FuncionarioController : Controller
    {
        private readonly AppDbContext _context;

        public FuncionarioController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult RegistroPonto()
        {
            var matricula = HttpContext.Session.GetInt32("Matricula");

            if (matricula == null)
                return RedirectToAction("Index", "Auth");

            var funcionario = _context.Funcionarios.FirstOrDefault(f => f.Matricula == matricula);

            if (funcionario == null)
                return NotFound();

            ViewBag.EntradaManha = funcionario.EntradaManha.ToString(@"hh\:mm");
            ViewBag.SaidaManha = funcionario.SaidaManha.ToString(@"hh\:mm");
            ViewBag.EntradaTarde = funcionario.EntradaTarde.ToString(@"hh\:mm");
            ViewBag.SaidaTarde = funcionario.SaidaTarde.ToString(@"hh\:mm");

            var registrosPonto = _context.RegistrosPonto
                .Where(r => r.MatriculaFuncionario == matricula && r.DataRegistro.Date == DateTime.Now.Date)
                .OrderBy(r => r.HoraBatida)
                .ToList();

            ViewBag.RegistrosPonto = registrosPonto;

            return View(funcionario);
        }

        [HttpPost]
        public IActionResult BaterPonto(string batida)
        {
            var matricula = HttpContext.Session.GetInt32("Matricula");
            if (matricula == null)
                return RedirectToAction("Index", "Auth");

            var funcionario = _context.Funcionarios.FirstOrDefault(f => f.Matricula == matricula);
            if (funcionario == null)
                return NotFound();

            string tipoBatidaLabel = batida switch
            {
                "EntradaManha" => "Entrada Manhã",
                "SaidaManha" => "Saída Manhã",
                "EntradaTarde" => "Entrada Tarde",
                "SaidaTarde" => "Saída Tarde",
                _ => null
            };

            if (tipoBatidaLabel == null)
                return BadRequest("Tipo de batida inválido.");

            bool batidaJaExiste = _context.RegistrosPonto.Any(r =>
                r.MatriculaFuncionario == funcionario.Matricula &&
                r.DataRegistro == DateTime.Now.Date &&
                r.TipoBatida == tipoBatidaLabel
            );

            if (batidaJaExiste)
            {
                TempData["Erro"] = $"Você já registrou a batida \"{tipoBatidaLabel}\" hoje.";
                return RedirectToAction("RegistroPonto");
            }

            var agora = DateTime.Now.TimeOfDay;

            TimeSpan? horaEsperada = batida switch
            {
                "EntradaManha" => funcionario.EntradaManha,
                "SaidaManha" => funcionario.SaidaManha,
                "EntradaTarde" => funcionario.EntradaTarde,
                "SaidaTarde" => funcionario.SaidaTarde,
                _ => null
            };

            if (horaEsperada == null)
            {
                TempData["Erro"] = "Horário esperado não definido para essa batida.";
                return RedirectToAction("RegistroPonto");
            }

            var inicioJanela = horaEsperada.Value.Subtract(TimeSpan.FromMinutes(15));
            var fimJanela = horaEsperada.Value.Add(TimeSpan.FromMinutes(15));

            if (agora < inicioJanela || agora > fimJanela)
            {
                TempData["Erro"] = $"Você só pode registrar \"{tipoBatidaLabel}\" entre {inicioJanela:hh\\:mm} e {fimJanela:hh\\:mm}.";
                return RedirectToAction("RegistroPonto");
            }

            var registroPonto = new RegistroPonto
            {
                MatriculaFuncionario = funcionario.Matricula,
                DataRegistro = DateTime.Now.Date,
                HoraBatida = agora,
                TipoBatida = tipoBatidaLabel
            };

            _context.RegistrosPonto.Add(registroPonto);
            _context.SaveChanges();

            return RedirectToAction("RegistroPonto");
        }

        public IActionResult Logoff()
        {
            HttpContext.Session.Remove("Matricula");
            return RedirectToAction("Index", "Auth");
        }
    }
}
