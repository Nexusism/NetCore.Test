using System;
using System.Collections.Generic;

namespace NetCore.Databases.Data
{
    public partial class User
    {
        public User()
        {
            UserRolesByUsers = new HashSet<UserRolesByUser>();
        }

        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool? IsMemberShipWithDrawn { get; set; }
        public DateTime JoinedUtcDate { get; set; }

        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}
