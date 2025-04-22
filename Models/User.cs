using System.ComponentModel.DataAnnotations;

namespace GitGenius.Models
{
    public enum UserRole { Student, Lecturer }
    public enum UserStatus { Pending, Approved, Rejected }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string IdNumber { get; set; }

        [Required]
        public string Password { get; set; }

        public UserRole Role { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Pending;

        public bool IsAdmin { get; set; } = false;

        public string? ApprovalFilePath { get; set; }
    }
}
