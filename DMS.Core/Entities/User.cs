using System.ComponentModel.DataAnnotations;

namespace DMS.Core.Entities
{
    public enum UserRole
    {
        Admin,
        User
    }
    public class User : BaseEntity
    {
        [Required]
        public string email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string password { get; set; }

        [Required]
        public string NID { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public virtual Workspace Workspace { get; set; }
    }
}
