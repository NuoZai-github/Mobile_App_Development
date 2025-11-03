using System.ComponentModel.DataAnnotations;

namespace Mobile_App_Develop.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        public string FirstName { get; set; } = string.Empty;
        
        public string LastName { get; set; } = string.Empty;
        
        public string StudentId { get; set; } = string.Empty;
        
        public string AvatarUrl { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime LastLoginAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}