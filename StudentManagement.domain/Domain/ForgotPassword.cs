using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StudentManagement.domain.Domain
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [Range(1000000000L, 9999999999999999L, ErrorMessage = "Enter a valid numeric phone number.")]
        [Display(Name = "Phone Number")]
        public long PhoneNumber { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [Display(Name = "New Password")]
        public string Password { get; set; }
    }
}