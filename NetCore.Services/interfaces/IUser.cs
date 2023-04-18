using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.interfaces
{
    public interface IUser
    {
        bool MatchTheUserInfo(LoginInfo login);

        User GetUserInfo(string userId);

        IEnumerable<UserRolesByUser> GetRolesOwnedByUser(string userId);

        /// <summary>
        /// [사용자 등록]
        /// </summary>
        /// <param name="register">사용자 등록 뷰모델</param>
        /// <returns></returns>
        int RegisterUser(RegisterInfo register);
    }
}
