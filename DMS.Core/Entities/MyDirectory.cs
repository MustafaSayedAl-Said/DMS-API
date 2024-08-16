using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Core.Entities
{
    public class MyDirectory : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Workspace")]
        public int WorkspaceId { get; set; }

        public virtual Workspace Workspace { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}
