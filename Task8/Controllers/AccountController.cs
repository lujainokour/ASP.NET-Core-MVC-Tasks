using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityCafeteriaSupply8.Models;

namespace UniversityCafeteriaSupply8.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(
                user, model.Password);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(
                    user, "User");

                if (roleResult.Succeeded)
                {
                    TempData["Message"] = "Account created. Please log in.";
                    return RedirectToAction("Login");
                }

                await _userManager.DeleteAsync(user);

                ModelState.AddModelError(
                    "", "Registration failed. Please try again.");

                return View(model);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, false, true);

            if (result.Succeeded) return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Login failed. Check your email and password or try again later.");
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}
