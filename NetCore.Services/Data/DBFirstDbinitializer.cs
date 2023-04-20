using NetCore.Data.Classes;
using NetCore.Services.interfaces;
using NetCore.Services.Svcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Data
{
    public class DBFirstDbinitializer
    {
        private DBFirstDbContext _context;
        private IPasswordHasher _hasher;
        public DBFirstDbinitializer(DBFirstDbContext context, IPasswordHasher hasher) 
        { 
            _context = context;
            _hasher = hasher;
        }

        /// <summary>
        /// 초기 데이터 입력
        /// </summary>
        public int PlantSeedData()
        {
            int rowAffected = 0;
            string userId = "sysadmin";
            string password = "1q2w3e$R";
            var passwordInfo = _hasher.SetPasswordInfo(userId, password);
            var utcNow = DateTime.UtcNow;

            _context.Database.EnsureCreated();
            if(!_context.Users.Any())
            {
                var users = new List<User>()
                {
                    new User()
                    {
                        UserId = userId.ToLower(),
                        UserName = "시스템관리자",
                        UserEmail = "sysadmin@exsis.co.kr",
                        RNGSalt = passwordInfo.RNGSalt,
                        GUIDSalt = passwordInfo.GUIDSalt,
                        PasswordHash = passwordInfo.PasswordHash,
                        AccessFailedCount = 0,
                        IsMemberShipWithDrawn = false,
                        JoinedUtcDate = utcNow
                    }
                };

                _context.Users.AddRange(users);
                rowAffected += _context.SaveChanges();
            }

            if (!_context.UserRoles.Any())
            {
                var userRoles = new List<UserRole>()
                {
                    new UserRole()
                    {
                        RoleId = "AssociateUser",
                        RoleName = "준사용자",
                        RolePriority = 1,
                        ModifiedUtcDate = utcNow
                    },
                    new UserRole()
                    {
                        RoleId = "GeneralUser",
                        RoleName = "일반사용자",
                        RolePriority = 2,
                        ModifiedUtcDate = utcNow
                    },
                    new UserRole()
                    {
                        RoleId = "SuperUser",
                        RoleName = "향상된사용자",
                        RolePriority = 3,
                        ModifiedUtcDate = utcNow
                    },
                    new UserRole()
                    {
                        RoleId = "SystemUser",
                        RoleName = "시스템관리자",
                        RolePriority = 4,
                        ModifiedUtcDate = utcNow
                    }
                };

                _context.UserRoles.AddRange(userRoles);
                rowAffected += _context.SaveChanges();
            }

            if (!_context.UserRolesByUsers.Any())
            {
                var userRolesByUsers = new List<UserRolesByUser>()
                {

                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "GeneralUser",
                        OwnedUtcDate = utcNow
                    },
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SuperUser",
                        OwnedUtcDate = utcNow
                    },
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SystemUser",
                        OwnedUtcDate = utcNow
                    }
                };

                _context.UserRolesByUsers.AddRange(userRolesByUsers);
                rowAffected += _context.SaveChanges();
            }

            return rowAffected;
        }
    }
}
