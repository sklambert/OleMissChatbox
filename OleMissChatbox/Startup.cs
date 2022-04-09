using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OleMissChatbox.Data;
using System;

namespace OleMissChatbox
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OleMissChatboxContext>();
              
            services.AddTransient<OleMissChatboxSeeder>();

            services.AddScoped<IOleMissChatboxRepository, OleMissChatboxRepository>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();
           
            services.AddMvc();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseEndpoints(cfg =>
            {
                cfg.MapControllerRoute(
                    "Default",
                    "/{controller}/{action}/{id?}",
                    new { controller = "Account", action = "Login" });

                cfg.MapRazorPages();
            });
        }
    }
}