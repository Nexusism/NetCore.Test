using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {
        private DBFirstDbContext _context;
        private IPasswordHasher _hasher;

        public UserService(DBFirstDbContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }
        #region private methods
        private IEnumerable<User> GetUserInfos()
        {
            return _context.Users.ToList();
            //return new List<User>()
            //{
            //    new User()
            //    {
            //        UserId = "ffpower",
            //        UserName = "이종국",
            //        UserEmail = "wanggoog@naver.com",
            //        Password = "123123"
            //    }
            //};
        }

        private User GetUserInfo(string userId, string password)
        {
            User user;

            // Lambda
            //user = _context.Users.Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();

            // FromSqlInterpolated , FromSqlRaw
            // 1. Table
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName, UserEmail, Password, IsMemberShipWithdrawn, JoinedUtcDate FROM dbo.[User]")
            //                    .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //                    .FirstOrDefault(); // userId와 password가 맞는 FromSqlRaw중 가장 첫번째 user 를 리턴

            // 2. VIEW
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName, UserEmail, Password, IsMemberShipWithdrawn, JoinedUtcDate FROM dbo.uvwUser")
            //        .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //        .FirstOrDefault(); // userId와 password가 맞는 FromSqlRaw중 가장 첫번째 user 를 리턴

            Console.WriteLine("userId = " + userId);
            Console.WriteLine("password = " + password);
            // 3. FUNCTION
            //user = _context.Users.FromSqlRaw($"SELECT UserId, UserName, UserEmail, Password, IsMemberShipWithdrawn, JoinedUtcDate FROM dbo.ufnUser('{userId}', '{password}')")
            //       .FirstOrDefault();

            // 4. STORED PROCEDURE
            user = _context.Users.FromSqlRaw($"EXEC dbo.uspCheckloginByUserId @p0, @p1", new[] { userId, password })
                .ToList()
                .FirstOrDefault();

            

            // 로그인 실패
            if ( user == null)
            {
                // 접속 실패 횟수 증가
                int rowAffected;

                // SQL문 직접 작성
                //rowAffected = _context.Database.ExecuteSqlInterpolated($"Update dbo.[User] SET AccessFailedCount += 1 WHERE UserId={userId}");

                // STORED PROCEDURE
                rowAffected = _context.Database.ExecuteSqlRaw($"EXEC dbo.FailedLoginByUserId @p0", new[] { userId });
            }

            return user;
        }

        private bool checkTheUserInfo(string userId, string password)
        {
            //return GetUserInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any(); // Any는 리스트일때만 사용이 가능
            return GetUserInfo(userId, password) != null ? true : false;
        }

        //private User GetUserInfo(string userId)
        //{
        //    return _context.Users.Where(u => u.UserId.Equals)
        //}

        private User GetUserInfo(string userId)
        {
            return _context.Users.Where(u => u.UserId.Equals(userId)).FirstOrDefault();
        }

        private IEnumerable<UserRolesByUser> GetUserRolesByUserInfos(string userId)
        {
            var userRolesByUserInfos = _context.UserRolesByUsers.Where(uru => uru.UserId.Equals(userId)).ToList();
            foreach(var role in userRolesByUserInfos)
            {
                role.UserRole = GetUserRole(role.RoleId);
            }
            return userRolesByUserInfos.OrderByDescending(uru => uru.UserRole.RolePriority);
        }

        private UserRole GetUserRole(string roleId)
        {
            return _context.UserRoles.Where(ur => ur.RoleId.Equals(roleId)).FirstOrDefault();
        }

        private int RegisterUser(RegisterInfo register)
        {
            var utcNow = DateTime.UtcNow;
            var passwordInfo = _hasher.SetPasswordInfo(register.UserId, register.Password);
            var user = new User()
            {
                UserId = register.UserId,
                UserName = register.UserName,
                UserEmail = register.UserEmail,
                RNGSalt = passwordInfo.RNGSalt,
                GUIDSalt = passwordInfo.GUIDSalt,
                PasswordHash = passwordInfo.PasswordHash,
                AccessFailedCount = 0,
                IsMemberShipWithDrawn = false,
                JoinedUtcDate = utcNow

            };

            var userRolesByUser = new UserRolesByUser()
            {
                UserId = register.UserId,
                RoleId = "AssociateUser",
                OwnedUtcDate = utcNow

            };

            _context.Add(user);
            _context.Add(userRolesByUser);

            return _context.SaveChanges();
        }
        #endregion

        bool IUser.MatchTheUserInfo(LoginInfo login)
        {
            
            //return checkTheUserInfo(login.UserId, login.Password);
            var user = _context.Users.Where(u => u.UserId.Equals(login.UserId)).FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            return _hasher.CheckThePasswordInfo(login.UserId, login.Password, user.RNGSalt, user.GUIDSalt, user.PasswordHash);
        }

        User IUser.GetUserInfo(string userId)
        {
            return GetUserInfo(userId);
        }

        IEnumerable<UserRolesByUser> IUser.GetRolesOwnedByUser(string userId)
        {
            return GetUserRolesByUserInfos(userId);
        }

        int IUser.RegisterUser(RegisterInfo register)
        {
            return RegisterUser(register);
        }
    }
}
