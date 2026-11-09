using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc_task6.Data;
using mvc_task6.Models;

namespace mvc_task6.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Product> items = _context.Products.ToList();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Suppliers.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product item)
        {
            if (!_context.Categories.Any(c => c.Id == item.CategoryId))
                ModelState.AddModelError("CategoryId", "Choose a category first.");
            if (!_context.Suppliers.Any(s => s.Id == item.SupplierId))
                ModelState.AddModelError("SupplierId", "Choose a supplier first.");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                ViewBag.Suppliers = new SelectList(_context.Suppliers.ToList(), "Id", "Name");
                return View(item);
            }

            _context.Products.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
