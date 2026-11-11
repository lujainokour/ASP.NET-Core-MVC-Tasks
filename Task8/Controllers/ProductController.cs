using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteriaSupply8.Data;
using UniversityCafeteriaSupply8.Models;

namespace UniversityCafeteriaSupply8.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.OrderBy(p => p.Name).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Create()
        {
            return View(new Product());
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Quantity")] Product product)
        {
            ValidateProduct(product);
            if (!ModelState.IsValid) return View(product);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Price,Quantity")] Product model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ValidateProduct(model);
            model.Id = id;
            if (!ModelState.IsValid) return View(model);
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            if (await _context.RequestItems.AnyAsync(i => i.ProductId == id))
            {
                ModelState.AddModelError("", "This product is used in a request and cannot be deleted.");
                return View("Delete", product);
            }
            _context.Products.Remove(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "This product is used in a request and cannot be deleted.");
                return View("Delete", product);
            }
            return RedirectToAction("Index");
        }

        private void ValidateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                ModelState.AddModelError("Name", "Product name is required.");
            if (product.Price <= 0 || product.Price > 1000000 || decimal.Round(product.Price, 2) != product.Price)
                ModelState.AddModelError("Price", "Enter a price between 0.01 and 1000000, with up to two decimal places.");
            if (product.Quantity < 0)
                ModelState.AddModelError("Quantity", "Quantity cannot be negative.");
            product.Description = product.Description ?? "";
        }
    }
}
