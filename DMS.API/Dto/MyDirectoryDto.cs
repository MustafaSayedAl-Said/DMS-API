using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.Dto
{
    public class MyDirectoryDto
    {
        public int id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Workspace")]
        public int WorkspaceId { get; set; }
    }
}
