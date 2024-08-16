using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Core.Entities
{
    public class Document : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("MyDirectory")]
        public int DirectoryId { get; set; }

        public virtual MyDirectory MyDirectory { get; set; }
    }
}
