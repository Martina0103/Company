using Company.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;


namespace Company.ViewModels
{
    public class EmployeeJobViewModel
    {
        public IList<Employee> Employees { get; set; }
        public SelectList JobTitles { get; set; }
        public string EmployeeJobTitle { get; set; }
        public string SearchString { get; set; }
    }
}
