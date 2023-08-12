using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cafe.Data;
using Cafe.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Cafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class GaleriController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GaleriController(ApplicationDbContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Galeri
        public async Task<IActionResult> Index()
        {
            return View(await _context.Galeris.ToListAsync());
        }



        // GET: Admin/Galeri/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Galeri/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image")] Galeri galeri)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(_webHostEnvironment.WebRootPath, @"Site\menu");
                    var ext = Path.GetExtension(files[0].FileName);
                    if (galeri.Image != null)
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, galeri.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + ext), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    galeri.Image = @"\Site\menu\" + fileName + ext;

                }

                _context.Add(galeri);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(galeri);
        }


        // GET: Admin/Galeri/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galeri = await _context.Galeris
                .FirstOrDefaultAsync(m => m.Id == id);
            if (galeri == null)
            {
                return NotFound();
            }

            return View(galeri);
        }

        // POST: Admin/Galeri/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var galeri = await _context.Galeris.FindAsync(id);
            if (galeri.Image != null)
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, galeri.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.Galeris.Remove(galeri);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GaleriExists(int id)
        {
            return _context.Galeris.Any(e => e.Id == id);
        }
    }
}
