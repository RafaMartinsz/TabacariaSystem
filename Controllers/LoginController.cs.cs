using Microsoft.AspNetCore.Mvc;

namespace TabacariaSystem.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Entrar(string usuario, string senha)
        {
            if (usuario == "admin" && senha == "123")
            {
                HttpContext.Session.SetString("UsuarioLogado", usuario);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Usuário ou senha inválidos";

            return View("Index");
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }
    }
}