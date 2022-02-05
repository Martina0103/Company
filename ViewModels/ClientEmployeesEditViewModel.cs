using Company.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.ViewModels
{
    public class ClientEmployeesEditViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<int> SelectedEmployees { get; set; }
        public IEnumerable<SelectListItem> EmployeeList { get; set; }
    }
}
