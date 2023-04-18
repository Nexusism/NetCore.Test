using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;

namespace NetCore.Web.Controllers
{
    [Authorize(Roles = "GeneralUser ,SuperUser, SystemUser")]
    public class DataController : Controller
    {
        private IDataProtector _protector;

        public DataController(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("NetCore.Data.v1");
        }
        [HttpGet]
        public IActionResult AES()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AES(AESInfo aes)
        {
            Console.WriteLine("AES POST 컨트롤러 시작");
            System.Diagnostics.Debug.WriteLine("AES Post 컨트롤러 " + ModelState);
            System.Diagnostics.Debug.WriteLine("AES Post 컨트롤러 " + ModelState.IsValid);
            System.Diagnostics.Debug.WriteLine("AES id = " + aes.UserId);
            System.Diagnostics.Debug.WriteLine("AES pw = " + aes.Password);
            

            string message = string.Empty;
            if (ModelState.IsValid)
            {
                string userInfo = aes.UserId + aes.Password;
                aes.EncUserInfo = _protector.Protect(userInfo); // 암호화 정보
                aes.DecUserInfo = _protector.Unprotect(aes.EncUserInfo); // 복호화 정보
                ViewData["Message"] = "암복호화가 성공적으로 이루어졌습니다.";

                return View(aes);
            }
            else
            {
                message = "암복호화를 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(aes);
        }
    }
}
