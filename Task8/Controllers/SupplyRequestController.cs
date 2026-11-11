using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteriaSupply8.Data;
using UniversityCafeteriaSupply8.Models;

namespace UniversityCafeteriaSupply8.Controllers
{
    [Authorize]
    public class SupplyRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupplyRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
        }

        public async Task<IActionResult> Index()
        {
            var requests = _context.SupplyRequests.AsQueryable();
            if (!IsAdmin())
            {
                string userId = _userManager.GetUserId(User)!;
                requests = requests.Where(r => r.UserId == userId);
            }
            return View(await requests.OrderByDescending(r => r.Id).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            return View(await GetForm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestFormViewModel model)
        {
            var items = await GetItems(model);
            if (!ModelState.IsValid) return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();
            var request = new SupplyRequest
            {
                UserId = _userManager.GetUserId(User)!,
                RequestDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = items.Sum(i => i.Price * i.Quantity)
            };
            _context.SupplyRequests.Add(request);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                item.SupplyRequestId = request.Id;
                _context.RequestItems.Add(item);
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction("Details", new { id = request.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.SupplyRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (!IsAdmin() && request.UserId != _userManager.GetUserId(User)) return Forbid();

            var user = await _userManager.FindByIdAsync(request.UserId);
            var model = new RequestDetailsViewModel
            {
                Request = request,
                FullName = user?.FullName ?? "",
                Items = await (
                    from item in _context.RequestItems
                    join product in _context.Products on item.ProductId equals product.Id
                    where item.SupplyRequestId == id
                    select new RequestProductViewModel
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = item.Price,
                        Quantity = item.Quantity
                    }).ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var request = await _context.SupplyRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (request.UserId != _userManager.GetUserId(User)) return Forbid();
            if (request.Status != "Pending") return BadRequest("Only pending requests can be edited.");
            return View(await GetForm(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RequestFormViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var request = await _context.SupplyRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (request.UserId != _userManager.GetUserId(User)) return Forbid();
            if (request.Status != "Pending") return BadRequest("Only pending requests can be edited.");

            model.Id = id;
            var items = await GetItems(model);
            if (!ModelState.IsValid) return View(model);

            var oldItems = await _context.RequestItems.Where(i => i.SupplyRequestId == id).ToListAsync();
            _context.RequestItems.RemoveRange(oldItems);
            foreach (var item in items)
            {
                item.SupplyRequestId = id;
                _context.RequestItems.Add(item);
            }
            request.TotalPrice = items.Sum(i => i.Price * i.Quantity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.SupplyRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (request.UserId != _userManager.GetUserId(User)) return Forbid();
            if (request.Status != "Pending") return BadRequest("Only pending requests can be deleted.");
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var request = await _context.SupplyRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (request.UserId != _userManager.GetUserId(User)) return Forbid();
            if (request.Status != "Pending") return BadRequest("Only pending requests can be deleted.");
            _context.SupplyRequests.Remove(request);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var request = await _context.SupplyRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (request.Status != "Pending") return BadRequest("This request has already been reviewed.");

            var items = await _context.RequestItems.Where(i => i.SupplyRequestId == id)
                .OrderBy(i => i.ProductId).ToListAsync();
            foreach (var item in items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                {
                    TempData["Error"] = "Not enough stock to approve this request.";
                    return RedirectToAction("Details", new { id });
                }
                product.Quantity -= item.Quantity;
            }
            request.Status = "Approved";
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction("Details", new { id });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var request = await _context.SupplyRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (request.Status != "Pending") return BadRequest("This request has already been reviewed.");
            request.Status = "Rejected";
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction("Details", new { id });
        }

        private async Task<RequestFormViewModel> GetForm(int id = 0)
        {
            var model = new RequestFormViewModel
            {
                Id = id,
                Products = await _context.Products.OrderBy(p => p.Name)
                    .Select(p => new RequestProductViewModel
                    {
                        ProductId = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        AvailableQuantity = p.Quantity
                    }).ToListAsync()
            };
            if (id != 0)
            {
                var items = await _context.RequestItems.Where(i => i.SupplyRequestId == id).ToListAsync();
                foreach (var product in model.Products)
                {
                    product.Quantity = items.FirstOrDefault(i => i.ProductId == product.ProductId)?.Quantity ?? 0;
                }
            }
            return model;
        }

        private async Task<List<RequestItem>> GetItems(RequestFormViewModel model)
        {
            var items = new List<RequestItem>();
            var products = await _context.Products.ToDictionaryAsync(p => p.Id);
            var selectedIds = new HashSet<int>();

            foreach (var row in model.Products)
            {
                if (!selectedIds.Add(row.ProductId) || !products.TryGetValue(row.ProductId, out var product))
                {
                    ModelState.AddModelError("", "Invalid product selection.");
                    continue;
                }
                row.Name = product.Name;
                row.Price = product.Price;
                row.AvailableQuantity = product.Quantity;
                if (row.Quantity < 0 || row.Quantity > product.Quantity || row.Quantity > 1000000)
                {
                    ModelState.AddModelError("", "Enter a valid quantity for " + product.Name + ".");
                    continue;
                }
                if (row.Quantity > 0)
                {
                    items.Add(new RequestItem
                    {
                        ProductId = product.Id,
                        Quantity = row.Quantity,
                        Price = product.Price
                    });
                }
            }
            if (items.Count == 0) ModelState.AddModelError("", "Select at least one product by entering a quantity.");
            return items;
        }
    }
}
