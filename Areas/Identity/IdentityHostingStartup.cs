using System;
using Company.Areas.Identity.Data;
using Company.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Company.Areas.Identity.IdentityHostingStartup))]
namespace Company.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<CompanyContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("CompanyContext")));

            });
        }
    }
}