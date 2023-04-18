using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.Classes
{
    public class UserRole
    {
        [Key]
        public string RoleId { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public byte RolePriority { get; set; }
        public DateTime ModifiedUtcDate { get; set; }

        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}
