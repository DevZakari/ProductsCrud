using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CrudProducts.Data;
using CrudProducts.Controllers;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CrudProducts
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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
       {
           options.LoginPath = "/Admin/Login"; // Spécifiez ici le chemin vers votre page de connexion
       });
            // Ajouter le service Memory Cache
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddLogging(logging =>
            {
                logging.ClearProviders(); // Supprimez les autres fournisseurs de journalisation (facultatif)
                logging.AddConsole();     // Ajoutez le fournisseur Console
                logging.AddFile("logs/app.log"); // Ajoutez le fournisseur de fichiers
            });
            services.AddRazorPages();
            services.AddTransient<CategoriesController>();
            services.AddTransient<ProductsController>();
            services.AddSession();

            services.AddDbContext<CrudProductsContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("CrudProductsContext")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

       


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();
            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/Admin/Login");
                    return Task.CompletedTask;
                });
                endpoints.MapRazorPages();

     
            });

        }
    }
}
