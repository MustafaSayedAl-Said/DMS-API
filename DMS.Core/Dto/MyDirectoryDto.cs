using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Core.Dto
{
    public class MyDirectoryDto
    {
        public int id { get; set; }

        public string Name { get; set; }

        public int WorkspaceId { get; set; }
    }
}
