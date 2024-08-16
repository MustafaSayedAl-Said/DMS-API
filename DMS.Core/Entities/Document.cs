using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
