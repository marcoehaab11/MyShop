using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.DataAccess.Implementation;
using myshop.Entities.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using myshop.DataAccess.DbInitializer;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<ApplicationDbContext>(op=>op.UseSqlServer
(builder.Configuration.GetConnectionString("DefaultConnection") 
));
builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

builder.Services.AddIdentity<IdentityUser,IdentityRole>
    (op=>op.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromDays(4) )
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders().AddDefaultUI();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitialier,DbInitialier>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:SecretKey").Get<string>();
SeedDb();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
//for RazorPages
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Customer",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.Run();

void SeedDb()
{
    using(var scope =app.Services.CreateScope())
    {
        var dbInitalizer=scope.ServiceProvider.GetRequiredService<IDbInitialier>();
        dbInitalizer.Initialize();
    }
}