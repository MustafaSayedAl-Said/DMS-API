using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
