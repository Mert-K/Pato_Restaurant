using Cafe.Data;
using Cafe.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toast;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IToastNotification toast,IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _db = db;
            _toast = toast;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var menu = _db.Menus.Where(m=>m.Ozel==true).ToList();
            return View(menu);
        }
		
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact([Bind("Id,Name,Email,Telefon,Mesaj,Tarih")] Contact contact)
        {
            if(ModelState.IsValid)
            {
                contact.Tarih = DateTime.Now;
                _db.Add(contact);
                await _db.SaveChangesAsync();

				ToastrOptions toastrOptions = new ToastrOptions()
				{
					PositionClass = ToastPositions.TopRight,
					CloseButton = true,
					ProgressBar = true,
					TimeOut = 7000,
					Title = "İşlem Başarılı"
				};
				_toast.AddSuccessToastMessage("Teşekkür ederiz, Mesajınız İletildi.", toastrOptions);

				return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

		public IActionResult CategoryDetails(int? id)
        {
            var menu = _db.Menus.Where(m => m.CategoryID == id).ToList();
            ViewBag.KategoriId = id;
            return View(menu);
        }

        public IActionResult Blog()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Blog([Bind("Id,Title,Name,Email,Image,Onay,Mesaj,Tarih")] Blog blog)
        {
            blog.Tarih = DateTime.Now;
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(_webHostEnvironment.WebRootPath, @"Site\menu");
                    var ext = Path.GetExtension(files[0].FileName);
                    if (blog.Image != null)
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, blog.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + ext), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    blog.Image = @"\Site\menu\" + fileName + ext;

                }
                _db.Add(blog);
                await _db.SaveChangesAsync();

                ToastrOptions toastrOptions = new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopRight,
                    CloseButton = true,
                    ProgressBar = true,
                    TimeOut = 7000,
                    Title = "İşlem Başarılı"
                    //buraya istenirse daha fazla özellik eklenebilir.
                };
                _toast.AddSuccessToastMessage("Teşekkür ederiz, Yorumunuz İletildi. Yorumunuz onaylandığında Blog Sayfasında Görebilirsiniz.", toastrOptions);

                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        public IActionResult About()
        {
            var about = _db.Abouts.ToList();
            return View(about);
        }

        public IActionResult Galeri()
        {
            var galeri = _db.Galeris.ToList();
            return View(galeri);
        }

        [HttpGet]
        public IActionResult Rezervasyon()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rezervasyon([Bind("Id,Name,Email,TelefonNo,Sayi,Saat,Tarih")] Rezervasyon rezervasyon)
        {
            if (ModelState.IsValid)
            {
                _db.Add(rezervasyon);
                await _db.SaveChangesAsync();

                ToastrOptions toastrOptions = new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopRight,
                    CloseButton = true,
                    ProgressBar = true,
                    TimeOut = 7000,
                    Title = "İşlem Başarılı"
                    //buraya istenirse daha fazla özellik eklenebilir.
                };
                _toast.AddSuccessToastMessage("Teşekkür ederiz. Rezervasyon işlemi başarılı olarak gerçekleştirilmiştir.", toastrOptions);

                return RedirectToAction(nameof(Index));
            }
            return View(rezervasyon);
        }

        public IActionResult Menu()
        {
            var menu = _db.Menus.ToList();
            return View(menu);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
