using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace UniversityCafeteria.Controllers;

public class SupplyController : Controller
{
    private static readonly string[] ProductNames =
        ["Water", "Orange Juice", "Coffee", "Sandwich", "Chocolate"];

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.Categories = new[] { "Drinks", "Food", "Snacks", "Hot Drinks", "Cold Drinks" };
        return View();
    }

    [HttpGet]
    public IActionResult Products()
    {
        ViewData["Title"] = "Cafeteria Products";
        ViewData["ManagerName"] = "Ahmad Ali";
        ViewData["NumberOfProducts"] = ProductNames.Length;
        ViewData["Products"] = ProductNames;
        return View();
    }

    [HttpGet]
    public IActionResult Suppliers()
    {
        ViewBag.PageTitle = "Our Suppliers";
        ViewBag.Suppliers = new[] { "Fresh Food Company", "Happy Drinks Company", "Daily Supplies Company" };
        ViewBag.NumberOfSuppliers = 3;
        return View();
    }

    [HttpGet]
    public IActionResult CreateRequest()
    {
        ViewData["Products"] = ProductNames;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitRequest()
    {
        var employeeName = Request.Form["EmployeeName"].ToString().Trim();
        var productName = Request.Form["ProductName"].ToString();
        var quantityText = Request.Form["Quantity"].ToString();
        var requiredDateText = Request.Form["RequiredDate"].ToString();

        if (string.IsNullOrWhiteSpace(employeeName))
            ModelState.AddModelError("EmployeeName", "Employee name is required.");
        if (!ProductNames.Contains(productName))
            ModelState.AddModelError("ProductName", "Please select a valid product.");
        if (!int.TryParse(quantityText, out var quantity) || quantity <= 0)
            ModelState.AddModelError("Quantity", "Quantity must be a positive whole number.");
        if (!DateOnly.TryParseExact(requiredDateText, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _))
            ModelState.AddModelError("RequiredDate", "Please select a valid required date.");

        if (!ModelState.IsValid)
        {
            ViewData["Products"] = ProductNames;
            ViewData["EmployeeName"] = employeeName;
            ViewData["ProductName"] = productName;
            ViewData["Quantity"] = quantityText;
            ViewData["RequiredDate"] = requiredDateText;
            return View();
        }

        TempData["Message"] = $"Supply request submitted successfully for {employeeName}: {quantity} x {productName}, required on {requiredDateText}.";
        return RedirectToAction("About");
    }

    [HttpGet]
    public IActionResult About() => View();
}
