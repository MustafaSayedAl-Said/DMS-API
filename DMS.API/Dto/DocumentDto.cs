using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.Dto
{
    public class DocumentDto
    {
        public int id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("MyDirectory")]
        public int DirectoryId { get; set; }
    }
}
