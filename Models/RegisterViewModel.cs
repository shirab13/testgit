using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GitGenius.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "ID number")]
        public string IdNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public UserRole Role { get; set; }

        [Display(Name = "Upload student permit")]
        public IFormFile? ApprovalFile { get; set; }
    }
}
