using Company.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;


namespace Company.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CompanyContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<CompanyContext>>()))
            {
                if (context.Employee.Any() || context.Branch.Any() || context.Client.Any())
                {
                    return; // DB has been seeded
                }

                context.Branch.AddRange(
                   new Branch {/*Id = 1,*/ Name = "Corporate", StartDate = DateTime.Parse("1980-3-7"), Headquarters = "New York City", Profit = 9000000 },
                    new Branch { /*Id = 2,*/ Name = "Scranton", StartDate = DateTime.Parse("1987-10-2"), Headquarters = "Scranton, Pennsylvania", Profit = 6500000 },
                   new Branch { /*Id = 3,*/ Name = "Stamford", StartDate = DateTime.Parse("1988-5-15"), Headquarters = "Stamford, Connecticut", Profit = 4320000 }

                );

                context.SaveChanges();

                context.Client.AddRange(
                    new Client {/*Id = 1*/ Name = "Dunmore High school", Location = "Dunmore" },
                    new Client { /*Id = 2*/ Name = "FedEx", Location = "New York" },
                    new Client { /*Id = 3,*/ Name = "Prestige Postal Company", Location = "Scranton" },
                    new Client { /*Id = 4*/ Name = "Apex Technology", Location = "Silicon Valley" },
                    new Client { /*Id = 5,*/ Name = "White Pages", Location = "Philadelphia" }
                );

                context.SaveChanges();

                context.Employee.AddRange(
                    new Employee
                    {
                        //Id =5,
                        FirstName= "Josh",
                        LastName= "Porter",
                        JobTitle= "Regional Manager",
                        Salary= 79000,
                        BirthDate= DateTime.Parse("1969-9-5"),
                        BranchId = context.Branch.Single(t => t.Name == "Stamford").Id
                    },

                     new Employee
                     {
                         //Id =4,
                         FirstName = "Dwight",
                         LastName = "Schrute",
                         JobTitle = "Sales Representative",
                         Salary = 75000,
                         BirthDate = DateTime.Parse("1970-1-20"),
                         BranchId = context.Branch.Single(t => t.Name == "Scranton").Id
                     },
                     new Employee
                     {
                         //Id =3,
                         FirstName = "Angela",
                         LastName = "Martin",
                         JobTitle = "Head of Accounting",
                         Salary = 68000,
                         BirthDate = DateTime.Parse("1974-11-11"),
                         BranchId = context.Branch.Single(t => t.Name == "Scranton").Id
                     },
                     new Employee
                     {
                         //Id =2,
                         FirstName = "Jim",
                         LastName = "Halpert",
                         JobTitle = "Sales Representative",
                         Salary = 72000,
                         BirthDate = DateTime.Parse("1978-10-1"),
                         BranchId = context.Branch.Single(t => t.Name == "Scranton").Id
                     },
                     new Employee
                     {
                         //Id =1,
                         FirstName = "Keren",
                         LastName = "Filippelli",
                         JobTitle = "Sales Representative",
                         Salary = 70000,
                         BirthDate = DateTime.Parse("1979-2-25"),
                         BranchId = context.Branch.Single(t => t.Name == "Stamford").Id
                     },
                     new Employee
                     {
                         //Id =6,
                         FirstName = "Andy",
                         LastName = "Bernard",
                         JobTitle = "Regional Director in Charge of Sales",
                         Salary = 72000,
                         BirthDate = DateTime.Parse("1973-1-24"),
                         BranchId = context.Branch.Single(t => t.Name == "Stamford").Id
                     },
                    new Employee
                    {
                        //Id =7,
                        FirstName = "David",
                        LastName = "Wallace",
                        JobTitle = "CFO",
                        Salary = 250000,
                        BirthDate = DateTime.Parse("1967-11-17"),
                        BranchId = context.Branch.Single(t => t.Name == "Corporate").Id
                    },
                     new Employee
                     {
                         //Id =8,
                         FirstName = "Michael",
                         LastName = "Scott",
                         JobTitle = "Regional Manager",
                         Salary = 85000,
                         BirthDate = DateTime.Parse("1965-3-15"),
                         BranchId = context.Branch.Single(t => t.Name == "Scranton").Id
                     }


                );

                context.SaveChanges();

                context.ClientEmployees.AddRange(

                    new ClientEmployee { EmployeeId = 8, ClientId = 1 },
                    new ClientEmployee { EmployeeId = 1, ClientId = 2 }, //Keren -> FedEx
                    new ClientEmployee { EmployeeId = 6, ClientId = 2 }, //Andy -> FedEx
                    new ClientEmployee { EmployeeId = 2, ClientId = 3 },
                    new ClientEmployee { EmployeeId = 4, ClientId = 3 },
                    new ClientEmployee { EmployeeId = 2, ClientId = 4 },
                    new ClientEmployee { EmployeeId = 4, ClientId = 4 },
                    new ClientEmployee { EmployeeId = 4, ClientId = 5 }

                );

                context.SaveChanges();

            }
        }
    }
}
