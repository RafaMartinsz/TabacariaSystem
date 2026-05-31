using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TabacariaSystem.Data;

namespace TabacariaSystem.Controllers
{
    [Authorize]
    public class VendasController : Controller
    {
        private readonly AppDbContext _context;

        public VendasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Vendas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vendas
     .Include(v => v.Cliente)
     .Include(v => v.Produto)
     .ToListAsync());
        }

        // GET: Vendas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Produto)
            .FirstOrDefaultAsync(m => m.Id == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // GET: Vendas/Create
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(
                _context.Clientes,
                "Id",
                "Nome"
            );

            // MOSTRA SOMENTE PRODUTOS COM ESTOQUE
            ViewBag.Produtos = new SelectList(
                _context.Produtos.Where(p => p.Quantidade > 0),
                "Id",
                "Nome"
            );

            return View();
        }

        // POST: Vendas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Vendas/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venda venda)
        {
            // Procura o produto no banco
            var produto = await _context.Produtos.FindAsync(venda.ProdutoId);

            // Verifica se o produto existe
            if (produto == null)
            {
                ModelState.AddModelError("", "Produto não encontrado.");

                // Recarrega os dropdowns
                ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome");
                ViewBag.Produtos = new SelectList(
                    _context.Produtos.Where(p => p.Quantidade > 0),
                    "Id",
                    "Nome"
                );

                return View(venda);
            }

            // Verifica se o estoque é suficiente
            if (produto.Quantidade < venda.Quantidade)
            {
                ModelState.AddModelError("",
                    $"Estoque insuficiente. Disponível: {produto.Quantidade}");

                // Recarrega os dropdowns
                ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome");

                ViewBag.Produtos = new SelectList(
                    _context.Produtos.Where(p => p.Quantidade > 0),
                    "Id",
                    "Nome"
                );

                return View(venda);
            }

            // Calcula valor total da venda
            venda.ValorTotal = produto.Preco * venda.Quantidade;

            // Atualiza estoque
            produto.Quantidade -= venda.Quantidade;

            // Salva data da venda
            venda.DataVenda = DateTime.Now;

            // Adiciona venda no banco
            _context.Vendas.Add(venda);

            // Salva tudo no banco
            await _context.SaveChangesAsync();

            // Volta para lista de vendas
            return RedirectToAction(nameof(Index));
       
        }
        // GET: Vendas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas.FindAsync(id);
            if (venda == null)
            {
                return NotFound();
            }
            return View(venda);
        }

        // POST: Vendas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,ProdutoId,Quantidade,ValorTotal,DataVenda")] Venda venda)
        {
            if (id != venda.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendaExists(venda.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venda);
        }

        // GET: Vendas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
    .Include(v => v.Cliente)
    .Include(v => v.Produto)
    .FirstOrDefaultAsync(m => m.Id == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // POST: Vendas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venda = await _context.Vendas.FindAsync(id);
            if (venda != null)
            {
                _context.Vendas.Remove(venda);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendaExists(int id)
        {
            return _context.Vendas.Any(e => e.Id == id);
        }
    }
}
