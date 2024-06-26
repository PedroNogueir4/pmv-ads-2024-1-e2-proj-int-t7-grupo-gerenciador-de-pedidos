﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Controllers
{
    public class ItemPedidosController : Controller
    {
        private readonly AppDbContext _context;

        public ItemPedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ItemPedidos
        public async Task<IActionResult> Index()
        {
            var mesaString = HttpContext.Session.GetString("mesa");

            if (string.IsNullOrEmpty(mesaString) || !int.TryParse(mesaString, out int mesa) || mesa <= 0)
            {
                return BadRequest("Mesa inválida ou não definida.");
            }

            var appDbContext = _context.ItemPedidos.Include(i => i.Pedido)
                                                   .ThenInclude(p => p.Status)
                                                   .Include(i => i.Produto)
                                                   .Where(i => i.Pedido.MesaId == mesa);

            return View(await appDbContext.ToListAsync());
        }

        // GET: ItemPedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemPedido = await _context.ItemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemPedido == null)
            {
                return NotFound();
            }

            return View(itemPedido);
        }

        // GET: ItemPedidos/Create
        public IActionResult Create()
        {
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id");
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "Id", "Descricao");
            return View();
        }

        // POST: ItemPedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Quantidade,ProdutoId,PedidoId")] ItemPedido itemPedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemPedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", itemPedido.PedidoId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "Id", "Descricao", itemPedido.ProdutoId);
            return View(itemPedido);
        }

        // GET: ItemPedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemPedido = await _context.ItemPedidos.FindAsync(id);
            if (itemPedido == null)
            {
                return NotFound();
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", itemPedido.PedidoId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "Id", "Descricao", itemPedido.ProdutoId);
            return View(itemPedido);
        }

        // POST: ItemPedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantidade,ProdutoId,PedidoId")] ItemPedido itemPedido)
        {
            if (id != itemPedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemPedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemPedidoExists(itemPedido.Id))
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
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", itemPedido.PedidoId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "Id", "Descricao", itemPedido.ProdutoId);
            return View(itemPedido);
        }

        // GET: ItemPedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemPedido = await _context.ItemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemPedido == null)
            {
                return NotFound();
            }

            return View(itemPedido);
        }

        // POST: ItemPedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemPedido = await _context.ItemPedidos.FindAsync(id);
            if (itemPedido != null)
            {
                _context.ItemPedidos.Remove(itemPedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemPedidoExists(int id)
        {
            return _context.ItemPedidos.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updateStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            pedido.StatusId = 5;

            await _context.SaveChangesAsync();
            return Redirect("/");
        }
    }
}
