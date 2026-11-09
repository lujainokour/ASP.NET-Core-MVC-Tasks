using Microsoft.AspNetCore.Mvc;
using mvc_task6.Data;
using mvc_task6.Models;

namespace mvc_task6.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Supplier> items = _context.Suppliers.ToList();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }

            _context.Suppliers.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
