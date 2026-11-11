using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteriaSupply8.Data;
using UniversityCafeteriaSupply8.Models;

namespace UniversityCafeteriaSupply8.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
            var model = new List<UserRoleViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserRoleViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    Role = string.Join(", ", roles)
                });
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            return View(new UserRoleViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? "User"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserRoleViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            model.Id = id;
            model.FullName = user.FullName;
            model.Email = user.Email ?? "";

            if (id == _userManager.GetUserId(User))
                ModelState.AddModelError("", "You cannot change your own role.");
            if (model.Role != "User" && model.Role != "Admin" && model.Role != "SuperAdmin")
                ModelState.AddModelError("Role", "Select a valid role.");
            if (!ModelState.IsValid) return View(model);

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (result.Succeeded) result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded) result = await _userManager.UpdateSecurityStampAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
                return View(model);
            }
            await transaction.CommitAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(new UserRoleViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? ""
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var model = new UserRoleViewModel { Id = id, FullName = user.FullName, Email = user.Email ?? "" };
            if (id == _userManager.GetUserId(User))
            {
                ModelState.AddModelError("", "You cannot delete your own account.");
                return View("Delete", model);
            }

            var requests = await _context.SupplyRequests.Where(r => r.UserId == id).ToListAsync();
            _context.SupplyRequests.RemoveRange(requests);
            await _context.SaveChangesAsync();
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
                return View("Delete", model);
            }
            await transaction.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
