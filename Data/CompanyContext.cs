using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Company.Models;
using Company.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Company.Data
{
    public class CompanyContext : IdentityDbContext<CompanyUser>
    {
        public CompanyContext(DbContextOptions<CompanyContext> options)
            : base(options)
        {
        }

        public DbSet<Company.Models.Client> Client { get; set; }

        public DbSet<Company.Models.Branch> Branch { get; set; }

        public DbSet<Company.Models.Employee> Employee { get; set; }

        public DbSet<Company.Models.ClientEmployee> ClientEmployees { get; set; }

        public DbSet<Company.Models.Intern> Intern { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}