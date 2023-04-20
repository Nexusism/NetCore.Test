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

        /// <summary>
        /// [사용자 정보수정을 위해 기존 사용자정보 검색]
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <returns></returns>
        UserInfo GetUserInfoForUpdate(string userId);

        /// <summary>
        /// [사용자 정보수정]
        /// </summary>
        /// <param name="user">사용자정보 뷰모델</param>
        /// <returns></returns>
        int UpdateUser(UserInfo user);

        /// <summary>
        /// [사용자 정보 수정 시 변경내용과 기존내용 비교]
        /// </summary>
        /// <param name="user">사용자정보 뷰모델</param>
        /// <returns></returns>
        bool CompareInfo(UserInfo user);

        /// <summary>
        /// 사용자 탈퇴
        /// </summary>
        /// <param name="user">탈퇴할 사용자 뷰모델</param>
        /// <returns></returns>
        int WithdrawnUser(WithdrawnInfo user);
    }
}
