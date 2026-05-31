using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TabacariaSystem.Models;
using TabacariaSystem.Data;

namespace TabacariaSystem.Controllers
{

    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // 1. TELA DE LOGIN (GET)
        public IActionResult Index()
        {
            return View();
        }

        // 2. AÇÃO DE LOGAR (POST)
        [HttpPost]
        public async Task<IActionResult> Entrar(string usuario, string senha)
        {
            var usuarioBanco = _context.Usuarios.FirstOrDefault(u => u.NomeUsuario == usuario);

            if (usuarioBanco != null)
            {
                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Usuario>();
                var resultado = hasher.VerifyHashedPassword(usuarioBanco, usuarioBanco.Senha, senha);

                if (resultado == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
                {
                    // Salva na Sessão
                    HttpContext.Session.SetString("UsuarioLogado", usuarioBanco.NomeUsuario);

                    // Cria o Cookie de Autenticação (Libera as outras classes)
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, usuarioBanco.NomeUsuario) };
                    var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidade));

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Erro = "Usuário ou senha inválidos";
            return View("Index");
        }

        // 3. AÇÃO DE LOGOUT (GET)
        public async Task<IActionResult> Sair()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        // 4. TELA DE CADASTRO (GET)
        public IActionResult Registrar()
        {
            return View();
        }

        // 5. AÇÃO DE CADASTRAR (POST)
        [HttpPost]
        public IActionResult Registrar(string usuario, string senha)
        {
            var usuarioExiste = _context.Usuarios.Any(u => u.NomeUsuario == usuario);
            if (usuarioExiste)
            {
                ViewBag.Erro = "Este nome de usuário já está em uso.";
                return View();
            }

            var novoUsuario = new Usuario { NomeUsuario = usuario };
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Usuario>();
            novoUsuario.Senha = hasher.HashPassword(novoUsuario, senha);

            _context.Usuarios.Add(novoUsuario);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // 6. LISTA DE USUÁRIOS (GET)
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Usuarios()
        {
            var listaUsuarios = _context.Usuarios.ToList();
            return View(listaUsuarios);
        }

        // 7. TELA DE CONFIRMAÇÃO DE EXCLUSÃO (GET)
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Delete(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // 8. AÇÃO DE CONFIRMAR EXCLUSÃO (POST) - BLINDADA
        [HttpPost, ActionName("Delete")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult DeleteConfirmed(int id)
        {
            // Pega o nome do usuário logado na sessão atualmente
            var usuarioLogado = HttpContext.Session.GetString("UsuarioLogado");

            var usuarioParaExcluir = _context.Usuarios.Find(id);

            if (usuarioParaExcluir != null)
            {
                // 1ª VALIDAÇÃO: Comparação segura ignorando maiúsculas e minúsculas
                bool ehOMesmoNome = string.Equals(usuarioParaExcluir.NomeUsuario, usuarioLogado, StringComparison.OrdinalIgnoreCase);

                if (ehOMesmoNome)
                {
                    TempData["ErroExclusao"] = "Segurança: Você não pode excluir o seu próprio usuário enquanto estiver logado!";
                    return RedirectToAction("Usuarios");
                }

                // Se passou da validação, pode excluir com segurança
                _context.Usuarios.Remove(usuarioParaExcluir);
                _context.SaveChanges();
            }

            return RedirectToAction("Usuarios");
        }
        // 9. TELA DE EDIÇÃO (GET)
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Edit(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.Senha = ""; // Limpa por segurança
            return View(usuario);
        }

        // 10. AÇÃO DE SALVAR A EDIÇÃO (POST)
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Edit(int id, string nomeUsuario, string novaSenha)
        {
            var usuarioBanco = _context.Usuarios.Find(id);
            if (usuarioBanco == null)
            {
                return NotFound();
            }

            var usuarioExiste = _context.Usuarios.Any(u => u.NomeUsuario == nomeUsuario && u.Id != id);
            if (usuarioExiste)
            {
                ViewBag.Erro = "Este nome de usuário já está em uso.";
                return View(usuarioBanco);
            }

            usuarioBanco.NomeUsuario = nomeUsuario;

            if (!string.IsNullOrEmpty(novaSenha))
            {
                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Usuario>();
                usuarioBanco.Senha = hasher.HashPassword(usuarioBanco, novaSenha);
            }

            _context.SaveChanges();
            return RedirectToAction("Usuarios");
        }
    }
}