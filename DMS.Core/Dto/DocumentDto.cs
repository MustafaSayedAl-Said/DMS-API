using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Core.Dto
{
    public class DocumentDto
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("MyDirectory")]
        public int DirectoryId { get; set; }

        public IFormFile DocumentContent { get; set; }
    }

    public class DocumentGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int DirectoryId { get; set; }

        public string DocumentContent { get; set; }

        public DateTime ModifyDate { get; set; }

        public bool IsPublic { get; set; }

        public bool IsDeleted { get; set; }

        public string OwnerName { get; set; }
    }
}
