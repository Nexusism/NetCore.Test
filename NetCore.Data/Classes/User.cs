using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.Classes
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string GUIDSalt { get; set; } = null!;
        public string RNGSalt { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int AccessFailedCount { get; set; }
        public bool? IsMemberShipWithDrawn { get; set; }
        public DateTime JoinedUtcDate { get; set; }

        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}
