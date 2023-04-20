using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetCore.Data.ViewModels
{
    public class ChangeInfo
    {
        /// <summary>
        /// 사용자명
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        /// <summary>
        /// true : 데이터 변경사항이 없을때, false : 변경사항이 있을때
        /// </summary>
        /// <param name="other">변경할 UserInfo</param>
        /// <returns></returns>
        public bool Equals(UserInfo other)
        {
            if(!string.Equals(UserName, other.UserName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!string.Equals(UserEmail, other.UserEmail, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}
