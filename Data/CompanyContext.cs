using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Company.Models;

namespace Company.Data
{
    public class CompanyContext : DbContext
    {
        public CompanyContext (DbContextOptions<CompanyContext> options)
            : base(options)
        {
        }

        public DbSet<Company.Models.Client> Client { get; set; }

        public DbSet<Company.Models.Branch> Branch { get; set; }

        public DbSet<Company.Models.Employee> Employee { get; set; }

        public DbSet<Company.Models.ClientEmployee> ClientEmployees { get; set; }       

        public DbSet<Company.Models.Intern> Intern { get; set; }
    }
}
