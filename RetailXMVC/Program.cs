using DataAccessObjectRetailX;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RepositoriesRetailX;
using DataAccessObject;
using Repositories;
using Microsoft.AspNetCore.HttpOverrides;

namespace RetailXMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            builder.Services.AddDbContext<RetailXContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("RetailX")));
            builder.Services.AddDbContext<Tenant0Context>((sp, options) =>
            {
                var http = sp.GetRequiredService<IHttpContextAccessor>();
                var tenantRepo = sp.GetRequiredService<ITenantRepository>();

                var user = http.HttpContext?.User;
                var tenantIdStr = user?.FindFirst("TenantId")?.Value;

                if (string.IsNullOrEmpty(tenantIdStr) || !int.TryParse(tenantIdStr, out var tenantId))
                {
                    // chưa có tenant
                    return;
                }

                var tenant = tenantRepo.GetTenantById(tenantId);
                if (tenant == null) return;

                var connStr = tenantRepo.BuildTenantConnectionString(tenant);
                Console.WriteLine("CONNECTION STRING IS "+connStr);
                options.UseSqlServer(connStr);
            });

            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
          
            builder.Services.AddScoped<SalaryDAO>();
            builder.Services.AddScoped<ISalaryRepository, SalaryRepository>();
          
            builder.Services.AddScoped<ReportRevenueDAO>();
            builder.Services.AddScoped<IReportRepository, ReportRepository>();
          
            builder.Services.AddScoped<TenantDAO>();
            builder.Services.AddScoped<ITenantRepository, TenantRepository>();
          
            builder.Services.AddScoped<StaffDAO>();
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();
          
            builder.Services.AddScoped<LogDAO>();
            builder.Services.AddScoped<ILogRepository, LogRepository>();

            builder.Services.AddScoped<ProductDAO>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            builder.Services.AddScoped<CategoryDAO>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            builder.Services.AddScoped<SupplierDAO>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            
            builder.Services.AddScoped<OrderDAO>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            builder.Services.AddScoped<OrderDetailDAO>();

            builder.Services.AddScoped<CustomerDAO>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

            builder.Services.AddScoped<OrderDetailDAO>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            builder.Services.AddScoped<StatisticDAO>();
            builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
          
            builder.Services.AddScoped<RequestDAO>();
            builder.Services.AddScoped<IRequestRepository, RequestRepository>();
          
            builder.Services.AddScoped<PurchaseOrderDAO>();
            builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseRepository>();

            builder.Services.AddScoped<PurchaseOrderDetailDAO>();
            builder.Services.AddScoped<IPurchaseOrderDetailRepository, PurchaseOrderDetailRepository>();

            builder.Services.AddScoped<InventoryDashboardDAO>();
            builder.Services.AddScoped<IInventoryDashBoardRepository, InventoryDashboardRepository>();

            builder.Services.AddScoped<SystemLogDAO>();
            builder.Services.AddScoped<ISystemLogRepository, SystemLogRepository>();

            builder.Services.AddScoped<UserLoginHistoryDAO>();
            builder.Services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("TenantLoggedIn", policy =>
                    policy.RequireClaim("IsTenantLogin", "True"));

            });

            builder.Services.AddSignalR();
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

            app.MapHub<SignalR.NotificationHub>("/notificationHub");
            app.Run();
        }
    }
}
