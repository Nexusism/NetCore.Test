using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.DataModels
{
    /// <summary>
    /// 사용자 소유권한
    /// </summary>
    public class UserRolesByUser
    {         // UserId, RoleId 복합키
        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public String UserId { get; set; }

        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public String RoleId { get; set; }

        [Required]
        public DateTime OwnedUtcDate { get; set; }

        public virtual User User { get; set; }

        public virtual UserRole UserRole { get; set; }
    }
}
