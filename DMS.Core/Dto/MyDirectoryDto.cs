using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Core.Dto
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
