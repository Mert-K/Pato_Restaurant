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
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenuController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Menu
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Menus.Include(m => m.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Menu/Create
        public IActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Image,Ozel,Price,CategoryID")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(files.Count>0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(_webHostEnvironment.WebRootPath, @"Site\menu");
                    var ext = Path.GetExtension(files[0].FileName);
                    if(menu.Image!=null)
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, menu.Image.TrimStart('\\'));
                        if(System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + ext), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    menu.Image = @"\Site\menu\" + fileName + ext;

                }

                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryID = new SelectList(_context.Categories, "Id", "Name", menu.CategoryID);
            return View(menu);
        }

        // GET: Admin/Menu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "Id", "Name", menu.CategoryID);
            return View(menu);
        }

        // POST: Admin/Menu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Image,Ozel,Price,CategoryID")] Menu menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(_webHostEnvironment.WebRootPath, @"Site\menu");
                    var ext = Path.GetExtension(files[0].FileName);
                    if (menu.Image != null)
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, menu.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + ext), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    menu.Image = @"\Site\menu\" + fileName + ext;

                }
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "Id", "Name", menu.CategoryID);
            return View(menu);
        }

        // GET: Admin/Menu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Admin/Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu.Image != null)
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, menu.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }
    }
}
