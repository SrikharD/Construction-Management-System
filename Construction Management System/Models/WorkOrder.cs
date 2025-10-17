// Created By: Srikhar Dogiparthy
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogiparthy_Spring25.Models
{
    public enum TaskStatus
    {
        [Display(Name = "Not Started")]
        NotStarted,

        [Display(Name = "In Progress")]
        InProgress,

        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "On Hold")]
        OnHold
    }

    [Table("WorkOrder")]
    public class WorkOrder
    {
        [Key]
        public int WorkOrderID { get; set; }

        [Display(Name = "Task Name")]
        [Required(ErrorMessage = "Task Name is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Task Name must be between 5 and 100 characters.")]
        public string TaskName { get; set; } 

        [Display(Name = "Description")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters.")]
        public string Description { get; set; } 

        [Required(ErrorMessage = "Status is required.")]
        public TaskStatus Status { get; set; } 

        [Display(Name = "Estimated End Date")]
        [DataType(DataType.Date)]
        public DateTime EstimatedEndDate { get; set; }

        // foreign key to property
        [ForeignKey("ProjectSite")]
        public int? ProjectSiteSiteID { get; set; }

        // Keep PropertyID for backward compatibility
        public int? PropertyID { get; set; }

        public virtual ProjectSite ProjectSite { get; set; }

        public virtual ICollection<WorkAllocation> WorkAllocations { get; set; }


    }
}
