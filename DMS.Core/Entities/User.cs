using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DMS.Core.Entities
{
    public class User : IdentityUser<int>
    {
        public string DisplayName { get; set; }

        public virtual Workspace Workspace { get; set; }
    }
}
