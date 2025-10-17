// Created By: Srikhar Dogiparthy
using System.ComponentModel.DataAnnotations;

namespace Dogiparthy_Spring25.Models
{
    public class SiteWorker : Person
    {
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Hourly Wage")]
        [Range(0, 1000, ErrorMessage = "Hourly Wage must be between 0 and 1000.")]
        public decimal HourlyWage { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        public virtual ICollection<WorkAllocation> WorkAllocations { get; set; }
    }
}
