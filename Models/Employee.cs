using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Company.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [NotMapped]
        public int Age
        {
            get
            {
                TimeSpan span = (TimeSpan)(DateTime.Now - BirthDate);
                double years = (double)span.TotalDays / 365.2425;
                return (int)years;
            }
        }
        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        [Range(50000, 500000)]
        public int? Salary { get; set; }

        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        public Branch Branch { get; set; }

        [Display(Name = "Branch")]
        public int? BranchId { get; set; }         //nadvoreshen kluc
        public ICollection<ClientEmployee> Clients { get; set; } //od junction tabelata

    }
}
