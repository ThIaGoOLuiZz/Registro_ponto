using Microsoft.AspNetCore.Mvc;
using RegistroDePonto.Models;

namespace RegistroDePonto.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Login(int matricula, string senha)
        {
            var funcionario = _context.Funcionarios
                .FirstOrDefault(f => f.Matricula == matricula && f.Senha == senha);

            if (funcionario == null)
            {
                TempData["ErroLogin"] = "Matrícula ou senha incorretas.";
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetInt32("Matricula", funcionario.Matricula);

            if (funcionario.Perfil == "Admin")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("RegistroPonto", "Funcionario");
        }   


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Auth");
        }
    }
}
