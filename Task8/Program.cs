using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteriaSupply8.Data;
using UniversityCafeteriaSupply8.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
    options.ValidationInterval = TimeSpan.Zero);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "User", "Admin", "SuperAdmin" };

    foreach (string role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            if (!result.Succeeded) throw new InvalidOperationException("Could not create role: " + role);
        }
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var superAdmins = await userManager.GetUsersInRoleAsync("SuperAdmin");
    string? email = builder.Configuration["BootstrapAdmin:Email"];
    string? password = builder.Configuration["BootstrapAdmin:Password"];

    if (superAdmins.Count == 0 && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
    {
        if (await userManager.FindByEmailAsync(email) != null)
            throw new InvalidOperationException("Choose a new email for the initial SuperAdmin account.");

        using var transaction = await context.Database.BeginTransactionAsync();
        var admin = new ApplicationUser { FullName = "Super Admin", UserName = email, Email = email };
        var result = await userManager.CreateAsync(admin, password);
        if (result.Succeeded) result = await userManager.AddToRoleAsync(admin, "SuperAdmin");
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(e => e.Description)));
        await transaction.CommitAsync();
    }
}

app.Run();
