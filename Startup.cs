using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using vega.Core;
using vega.Core.Models;
using vega.Persistence;

namespace vega {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {

            services.AddControllers ()
                .AddNewtonsoftJson ();

            // 1. Add Authentication Services
            services.AddAuthentication (options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer (options => {
                options.Authority = "https://kartzvega.auth0.com/";
                options.Audience = "https://api.vega.com";
            });

            // interface implementation using Dependency injection-- Transient type(seperate instance of repository for every use)
            // or Singleton  a single instance of repository during Application LifecYCLE. 
            //Scoped- asingle instance of repository foreach request

            services.Configure<PhotoSettings> (Configuration.GetSection ("PhotoSettings"));

            services.AddScoped<IVehicleRepository, VehicleRepository> ();

            services.AddScoped<IPhotoRepository, PhotoRepository> ();

            services.AddScoped<IUnitOfWork, UnitOfWork> ();

            services.AddAutoMapper (typeof (Startup));

            services.AddDbContext<VegaDbContext> (options => options.UseSqlServer (Configuration.GetConnectionString ("Default")));
            services.AddControllersWithViews ();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles (configuration => {
                configuration.RootPath = "ClientApp/dist/vega";
            });
        }

        // This method gets called by the rucdntime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {

            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler ("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            app.UseHttpsRedirection ();
            app.UseStaticFiles ();
            if (!env.IsDevelopment ()) {
                app.UseSpaStaticFiles ();
            }

            app.UseRouting ();

            // 2. Enable authentication middleware
            app.UseAuthentication ();
            app.UseAuthorization ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllerRoute (
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa (spa => {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment ()) {
                    spa.UseAngularCliServer (npmScript: "start");
                }
            });
        }
    }
}