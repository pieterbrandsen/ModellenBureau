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
using ModellenBureau.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModellenBureau.Models;
using Microsoft.AspNetCore.Http;
using ModellenBureau.Pages;

namespace ModellenBureau
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
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                    .AddDefaultTokenProviders()
                    .AddDefaultUI()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            // .AddRazorPagesOptions(options =>
            //{
            //    options.RootDirectory = "/Pages";
            //    options.Conventions.AddPageRoute("/Pages/UserDetails", "");
            //});
            //services.AddRouting();
            //services.AddScoped<FileModel>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                //endpoints.MapGet("/Pages/UserDetails/{UserId}", async context =>
                //{
                //    var UserId = context.Request.RouteValues["UserId"];
                //    await context.Response.WriteAsync($"{UserId}");
                //});
            });
            CreateRoles(serviceProvider).Wait();

        }
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string[] roleNames = { RoleNames.Admin, RoleNames.Customer, RoleNames.Model };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var admin = new ApplicationUser();
            admin.Email = "admin@admin.nl";
            admin.UserName = admin.Email;
            admin.PasswordHash = "AQAAAAEAACcQAAAAEC7vUlO2YPPjCAHhbTu4VWhY4fPF/p1lJqGE2X3tMjECNIaNaku8Eqo1exLzHAkwqw==";
            if (await UserManager.CreateAsync(admin) == IdentityResult.Success)
            {
                await UserManager.AddToRoleAsync(admin, RoleNames.Admin);
            }

            var result = System.IO.Directory.CreateDirectory(".wwwroot/uploads");
        }
    }
}
