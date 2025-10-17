// Created By: Srikhar Dogiparthy
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Dogiparthy_Spring25.Models
{
    public class CurrentOwner : Person
    {

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Occupation must be between 3 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only Letters and Spaces are allowed.")]
        public string Occupation { get; set; }

        [Display(Name = "Preferred Contact Method")]
        
       // UI uses DropDown options so no need of decorations
        public string PreferredContactMethod { get; set; }

        [Display(Name = "Preferred Payment Method")]
       // UI uses DropDown options so no need of decorations
        public string PreferredPaymentMethod { get; set; }

        // Navigation property for properties
        public virtual ICollection<ProjectSite> Properties { get; set; }

       

    }
}
