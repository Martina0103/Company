using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Company.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Range(100000, 9000000)]
        public int? Profit { get; set; }        
        public string Headquarters { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
