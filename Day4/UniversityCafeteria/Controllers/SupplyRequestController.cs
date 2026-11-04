using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UniversityCafeteria.Models;

namespace UniversityCafeteria.Controllers;

public class SupplyRequestController : Controller
{
    private static readonly List<SupplyRequest> Requests = new();

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.PreferredPriority = GetPreferredPriority();
        return View(GetDraft());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddItem(SupplyItem item, string priority)
    {
        if (string.IsNullOrWhiteSpace(item.ProductName))
            ModelState.AddModelError("ProductName", "Product name is required.");
        if (item.Quantity <= 0)
            ModelState.AddModelError("Quantity", "Quantity must be greater than zero.");
        if (item.UnitPrice <= 0)
            ModelState.AddModelError("UnitPrice", "Unit price must be greater than zero.");
        if (!IsValidPriority(priority))
            ModelState.AddModelError("Priority", "Choose a valid priority.");

        var draft = GetDraft();
        if (!ModelState.IsValid)
        {
            ViewBag.PreferredPriority = GetPreferredPriority();
            return View("Index", draft);
        }

        item.ProductName = item.ProductName.Trim();
        draft.Items.Add(item);
        draft.Priority = priority;
        Calculate(draft);
        SaveDraft(draft);
        TempData["Message"] = "Product added to the draft.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Review()
    {
        var draft = GetDraft();
        Calculate(draft);
        return View(draft);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmRequest()
    {
        var draft = GetDraft();
        if (draft.Items.Count == 0)
        {
            TempData["Error"] = "Add at least one product before confirming the request.";
            return RedirectToAction("Index");
        }

        Calculate(draft);
        draft.Status = "Confirmed";
        lock (Requests)
        {
            draft.RequestId = Requests.Count + 1;
            Requests.Add(draft);
        }
        HttpContext.Session.Remove("SupplyDraft");
        TempData["Message"] = "Request confirmed successfully.";
        return RedirectToAction("MyRequests");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ClearDraft()
    {
        HttpContext.Session.Remove("SupplyDraft");
        TempData["Message"] = "Draft cleared.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SavePreference(string priority)
    {
        if (!IsValidPriority(priority))
        {
            TempData["Error"] = "Choose a valid priority.";
            return RedirectToAction("Index");
        }

        Response.Cookies.Append("PreferredPriority", priority, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = Request.IsHttps
        });
        var draft = GetDraft();
        draft.Priority = priority;
        Calculate(draft);
        SaveDraft(draft);
        TempData["Message"] = "Your preferred request priority is " + priority + ".";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult MyRequests()
    {
        lock (Requests)
        {
            return View(Requests.ToList());
        }
    }

    [HttpGet]
    public IActionResult About() => View();

    private bool IsValidPriority(string? priority)
    {
        return priority == "Normal" || priority == "Urgent" || priority == "Emergency";
    }

    private string GetPreferredPriority()
    {
        var priority = Request.Cookies["PreferredPriority"];
        return IsValidPriority(priority) ? priority! : "Normal";
    }

    private SupplyRequest GetDraft()
    {
        var data = HttpContext.Session.GetString("SupplyDraft");
        if (data == null)
            return new SupplyRequest { Priority = GetPreferredPriority() };

        return JsonSerializer.Deserialize<SupplyRequest>(data)!;
    }

    private void SaveDraft(SupplyRequest draft)
    {
        HttpContext.Session.SetString("SupplyDraft", JsonSerializer.Serialize(draft));
    }

    private void Calculate(SupplyRequest request)
    {
        request.Subtotal = 0;
        foreach (var item in request.Items)
        {
            item.Total = Math.Round(item.Quantity * item.UnitPrice, 2);
            request.Subtotal += item.Total;
        }

        switch (request.Priority)
        {
            case "Urgent":
                request.PreparationFee = 5;
                break;
            case "Emergency":
                request.PreparationFee = 10;
                break;
            default:
                request.PreparationFee = 2;
                break;
        }

        if (request.Subtotal > 200)
            request.Discount = Math.Round(request.Subtotal * 0.15m, 2);
        else if (request.Subtotal > 100)
            request.Discount = Math.Round(request.Subtotal * 0.10m, 2);
        else
            request.Discount = 0;

        request.FinalTotal = request.Subtotal + request.PreparationFee - request.Discount;
    }
}
