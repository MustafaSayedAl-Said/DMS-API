using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Core.Dto
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Min Length is 3 characters")]
        public string DisplayName { get; set; }

        public string Paswword { get; set; }

        [Required]
        public string WorkspaceName { get; set; }
    }
}
