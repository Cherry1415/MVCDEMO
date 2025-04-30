using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using System.Text.Json;

namespace _06032025_MVCDAY1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped(typeof(IAdminRepository<>), typeof(AdminRepository<>));


            var RazorpayConfig = new RazorPayKeys();
            builder.Configuration.GetSection("RazorPay").Bind(RazorpayConfig);
            builder.Services.AddSingleton(RazorpayConfig);


            //add for session
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

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
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=DashBoard}/{action=HomeDashBoard}/{id?}");

            app.MapControllers(); // Required for Attribute Routing
            app.Run();
            
        }
    }
}
