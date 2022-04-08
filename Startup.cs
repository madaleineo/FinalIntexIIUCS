using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using UtahCarSafety.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UtahCarSafety.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.ML.OnnxRuntime;
using System.Net;

namespace UtahCarSafety
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                string constr = Environment.GetEnvironmentVariable("APPAUTH");
                options.UseMySql(constr);
            });




            services.AddDbContext<CrashesDbContext>(options =>
            {
                string constr = Environment.GetEnvironmentVariable("CRASHAUTH");
                options.UseMySql(constr);
            });


            services.AddScoped<ICrashesRepository, EFCrashesRepository>();



            // This forces best practices when creating a password
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddControllersWithViews();
            services.AddRazorPages();


            //services.AddIdentity<IdentityUser, IdentityRole>()
            //    .AddEntityFrameworkStores<AppIdentityDBContext>();

            services.AddDistributedMemoryCache();

            services.AddSession();


            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = Environment.GetEnvironmentVariable("CLIENTID");
                    options.ClientSecret = Environment.GetEnvironmentVariable("CLIENTSECRET");
                });


            //services.AddScoped<ICrashesRepository, EFCrashesRepository>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton(
                new InferenceSession("wwwroot/official_model.onnx")
            );


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            // CSP - will give errors, but won't mess up our own styles
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy-Report-Only", "default-src 'self'");
                await next();
            });



            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute("categorypage", "{bookCategory}/Page{pageNum}", new { Controller = "Home", action = "Index" });
                endpoints.MapRazorPages();

                endpoints.MapFallbackToPage("/Admin/{*catchall}", "/Admin/Index");


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "Paging",
                    pattern: "Page{pageNum}",
                    defaults: new { Controller = "Home", action = "Index", pageNum = 1 }
                    );

                //endpoints.MapControllerRoute("category", "{bookCategory}", new { Controller = "Home", action = "Index", pageNum = 1 });

                endpoints.MapDefaultControllerRoute();



            });

            //IdentitySeedData.EnsurePopulated(app);
        }
    }
}
