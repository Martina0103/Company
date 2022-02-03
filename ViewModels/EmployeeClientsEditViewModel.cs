using Company.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.ViewModels
{
    public class EmployeeClientsEditViewModel
    {
        public Employee Employee { get; set; }
        public IEnumerable<int> SelectedClients { get; set; }
        public IEnumerable<SelectListItem> ClientList { get; set; }
    }
}
