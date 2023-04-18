using NetCore.Services.Bridges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.interfaces
    // 인터페이스를 정의해놓아야 private메소드를 외부에서 사용가능
{
    public interface IPasswordHasher
    {
        string GetGUIDSalt();
        string GetRNGSalt();

        string GetPasswordHash(string userId, string password, string rngSalt, string guidSalt);

        bool CheckThePasswordInfo(string userId, string password, string rngSalt, string guidSalt, string passwordHashed);

        /// <summary>
        /// 사용자 등록 시 비밀번호 정보 지정을 위한 메서드
        /// </summary>
        /// <param name="userId">아이디</param>
        /// <param name="password">비밀번호</param>
        /// <returns></returns>
        PasswordHashInfo SetPasswordInfo(string userId, string password);
    }
}
