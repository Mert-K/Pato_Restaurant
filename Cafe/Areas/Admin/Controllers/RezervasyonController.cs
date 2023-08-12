using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cafe.Data;
using Cafe.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RezervasyonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RezervasyonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Rezervasyon
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rezervasyons.ToListAsync());
        }

        // GET: Admin/Rezervasyon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervasyon = await _context.Rezervasyons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rezervasyon == null)
            {
                return NotFound();
            }

            return View(rezervasyon);
        }

        // POST: Admin/Rezervasyon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rezervasyon = await _context.Rezervasyons.FindAsync(id);
            _context.Rezervasyons.Remove(rezervasyon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RezervasyonExists(int id)
        {
            return _context.Rezervasyons.Any(e => e.Id == id);
        }
    }
}
