# Task 8

University cafeteria supply requests using ASP.NET Core MVC, Identity, and SQL Server LocalDB.

- User: create requests and view their own requests.
- Admin: manage products and approve or reject requests.
- SuperAdmin: manage users and roles as well.

Open UniversityCafeteriaSupply8.slnx in Visual Studio. The project needs .NET 10 and SQL Server LocalDB. Database migrations run at startup.

For the first SuperAdmin account, open a PowerShell terminal in this folder and run:

```powershell
$env:BootstrapAdmin__Email = "superadmin@cafeteria.local"
$env:BootstrapAdmin__Password = "YOUR_STRONG_PASSWORD"
dotnet run --launch-profile https
```

Choose your own password with uppercase and lowercase letters, a number, and a symbol. The account is created only when no SuperAdmin exists. New registrations always get the User role.

Optional example products can be added after the first run:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d CafeteriaDb -E -b -i Data\SampleProducts.sql
```

Only pending requests can be edited or deleted by their owner. Approval checks and deducts stock. Products already used in requests cannot be deleted.
