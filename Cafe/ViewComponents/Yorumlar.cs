using Cafe.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Cafe.ViewComponents
{
    public class Yorumlar : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public Yorumlar(ApplicationDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            var yorum = _db.Blogs.Where(b => b.Onay == true).ToList();
            return View(yorum);
        }
    }
}
