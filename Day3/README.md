# University Cafeteria Day 3

Independent ASP.NET Core MVC project for the Day 3 task. Previous task folders remain unchanged.

Requires the .NET 10 SDK. From this directory, run:

```powershell
dotnet run --project UniversityCafeteria -- --urls http://localhost:5033
```

Open http://localhost:5033. The default route uses `SupplyController.Index`.

- `/Supply/Products`: five products, manager Ahmad Ali, and product count through ViewData and Bootstrap cards.
- `/Supply/Suppliers`: three suppliers through ViewBag.
- `/Supply/Index`: five categories rendered with foreach.
- `/Supply/CreateRequest`: GET form for employee, product, quantity, and date.
- `/Supply/SubmitRequest`: POST reads Request.Form, validates input, and redisplays errors with entered values preserved.
- Successful submissions set `TempData["Message"]` and redirect to `/Supply/About`. The notification is shown once.
- `/Supply/CheckQuantity?quantity=20`: bonus model binding and classification. Below 20 is Small Order, 20–50 is Medium Order, and above 50 is Large Order.

Navigation is defined in `Views/Shared/_MainLayout.cshtml`. No database or Entity Framework is used; requests are demonstrations and are not persisted.
