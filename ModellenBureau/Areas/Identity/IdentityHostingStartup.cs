using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModellenBureau.Data;

[assembly: HostingStartup(typeof(ModellenBureau.Areas.Identity.IdentityHostingStartup))]
namespace ModellenBureau.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                //services.AddDbContext<ApplicationDbContext>(options =>
                //    options.UseSqlServer(
                //        context.Configuration.GetConnectionString("AuthDbContextConnection")));


                //services.AddDefaultIdentity<ApplicationUser>(options =>
                //{
                //    options.SignIn.RequireConfirmedAccount = false;
                //    options.Password.RequireLowercase = false;
                //    options.Password.RequireUppercase = false;
                //})
                //.AddEntityFrameworkStores<ApplicationDbContext>();
            });
        }
    }
}