using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Core.Dto
{
    public class DocumentDto
    {

        [Required]
        [ForeignKey("MyDirectory")]
        public int DirectoryId { get; set; }

        public IFormFile DocumentContent { get; set; }
    }

    public class DocumentGetDto
    {
        public string Name { get; set; }

        public int DirectoryId { get; set; }

        public string DocumentContent { get; set; }
    }
}
