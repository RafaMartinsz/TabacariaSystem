using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TabacariaSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioLogado") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }
    }
}