using Company.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.ViewModels
{
    public class ClientNameViewModel
    {
        public IList<Client> Clients { get; set; }
        public SelectList Names { get; set; }
        public string ClientName { get; set; }
        public string SearchString { get; set; }
    }
}
