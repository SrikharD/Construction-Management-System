// Created By: Srikhar Dogiparthy
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogiparthy_Spring25.Models
{
    public enum SiteType
    {
        Residential,
        Commercial,
        Industrial,
        Agricultural
    }
    public class ProjectSite
    {
        [Key]
        public int SiteID { get; set; }

        [Display(Name = "Project Site Title")]
        [Required(ErrorMessage = "Project Site Title is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Property Title must be between 5 and 100 characters.")]
        public string SiteTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Location must be between 5 and 200 characters.")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Size (Sq Ft)")]
        [Range(1, 10000, ErrorMessage = "Size must be greater than 0 and less than 10000.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Enter a positive number.")]
        public int SizeSqFt { get; set; }

        [Display(Name = "Project Site Type")]
        [Required(ErrorMessage = "Project Site Type is required.")]
        public SiteType SiteType { get; set; }


        //// foreign key for mapping the owner
        //public int? CurrentOwnerPersonID { get; set; }

        [ForeignKey("CurrentOwner")]
        public int? CurrentOwnerPersonID { get; set; }

        public virtual CurrentOwner CurrentOwner { get; set; }

        public virtual ICollection<WorkOrder> WorkOrders { get; set; }  

    }
}
