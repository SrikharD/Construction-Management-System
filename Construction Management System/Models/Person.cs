// Created By: Srikhar Dogiparthy
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogiparthy_Spring25.Models
{
    public class Person
    {
        [Key]
        public int PersonID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "First Name should be between 3 and 50 characters.")] 
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name cannot have less than 3 or more than 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Phone Number.")]
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone is required.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address Line 1")]
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100, ErrorMessage = "Address Line 1 cannot exceed 100 characters.")]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        [StringLength(100, ErrorMessage = "Address Line 2 cannot exceed 100 characters.")]
        public string Address2 { get; set; }

        [StringLength(40, ErrorMessage = "City cannot exceed 40 characters.")]
        [Required(ErrorMessage = "City is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only Letters and Spaces are allowed.")]
        public string City { get; set; }

        [StringLength(40, ErrorMessage = "State cannot exceed 40 characters.")]
        [Required(ErrorMessage = "State is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only Letters and Spaces are allowed.")]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        [Required(ErrorMessage = "Zip Code is required.")]
        [StringLength(11, ErrorMessage = "Zip Code cannot exceed 11 characters.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Zip Code can only be a number.")]
        public string ZipCode { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        // for one-to-one relationship with user
        public string IdentityUserId { get; set; }

        // navigation property to allow user info
        public virtual IdentityUser IdentityUser { get; set; }
    }
}
