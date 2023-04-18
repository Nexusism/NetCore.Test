using System;
using System.Collections.Generic;

namespace NetCore.Databases.Data
{
    public partial class UserRole
    {
        public UserRole()
        {
            UserRolesByUsers = new HashSet<UserRolesByUser>();
        }

        public string RoleId { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public byte RolePriority { get; set; }
        public DateTime ModifiedUtcDate { get; set; }

        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}
