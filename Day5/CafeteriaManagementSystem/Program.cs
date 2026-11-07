using Microsoft.EntityFrameworkCore;
using CafeteriaManagementSystem.Models;
using CafeteriaManagementSystem.CodeFirstModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database First
builder.Services.AddDbContext<CafeteriaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("CafeteriaConnection")
    )
);

// Code First
builder.Services.AddDbContext<CafeteriaCodeFirstContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("CodeFirstConnection")
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
