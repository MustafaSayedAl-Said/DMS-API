using System.ComponentModel.DataAnnotations;

namespace DMS.Core.Dto
{
    public class UserDto
    {
        public int id { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string password { get; set; }

        [Required]
        public string NID { get; set; }

    }
}
