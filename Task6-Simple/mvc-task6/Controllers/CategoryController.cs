using Microsoft.AspNetCore.Mvc;
using mvc_task6.Data;
using mvc_task6.Models;

namespace mvc_task6.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Category> items = _context.Categories.ToList();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }

            _context.Categories.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
