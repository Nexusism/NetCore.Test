using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetCore.Data.ViewModels
{
    public class RegisterInfo
    {
        [Required(ErrorMessage = "사용자 ID를 입력하세요.")]
        [MinLength(6, ErrorMessage = "사용자 ID는 최소 6자 이상 입력하세요.")]
        [Display(Name = "사용자ID")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "사용자명을 입력하세요.")]
        [Display(Name = "사용자명")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "이메일을 입력하세요.")]
        [Display(Name = "이메일")]
        public string UserEmail { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "비밀번호를 입력하세요.")]
        [MinLength(6, ErrorMessage = "비밀번호는 최소 6자 이상 입력하세요.")]
        [Display(Name = "비밀번호")]
        public string Password { get; set; }
    }
}
