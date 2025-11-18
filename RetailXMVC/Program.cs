using DataAccessObjectRetailX;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RepositoriesRetailX;
using DataAccessObject;
using Repositories;

namespace RetailXMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<RetailXContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("RetailX")));
            builder.Services.AddDbContext<Tenant0Context>(options =>
options.UseSqlServer("Server=localhost;Database=Tenant_0;Trusted_Connection=True;TrustServerCertificate=True;"));
            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<StaffDAO>();
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();
            builder.Services.AddScoped<LogDAO>();
            builder.Services.AddScoped<ILogRepository, LogRepository>();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

            //Build
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
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
