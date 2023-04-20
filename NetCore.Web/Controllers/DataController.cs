using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Web.Extensions;

namespace NetCore.Web.Controllers
{
    [Authorize(Roles = "GeneralUser ,SuperUser, SystemUser")]
    public class DataController : Controller
    {
        private IDataProtector _protector;
        private HttpContext _context;
        private string _sessionKeyCartName = "_sessionCartKey";

        public DataController(IHttpContextAccessor accessor, IDataProtectionProvider provider)
        {
            _context = accessor.HttpContext;
            _protector = provider.CreateProtector("NetCore.Data.v1");
        }

        #region private methods
        private List<ItemInfo> GetCartInfos(ref string message)
        {
            var cartInfos = _context.Session.Get<List<ItemInfo>>(key: "_sessionCartKey");

            if(cartInfos == null || cartInfos.Count() < 1)
            {
                message = "장바구니에 담긴 상품이 없습니다.";
            }
            return cartInfos;
        }

        private void SetCartInfos(ItemInfo item, List<ItemInfo> cartInfos = null)
        {
            if(cartInfos == null)
            {
                cartInfos = _context.Session.Get<List<ItemInfo>>(_sessionKeyCartName);

                if(cartInfos == null)
                {
                    cartInfos = new List<ItemInfo>();
                }
            }

            cartInfos.Add(item);

            _context.Session.Set<List<ItemInfo>>(_sessionKeyCartName, cartInfos);
        }
        #endregion

        #region AES
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
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCart()
        {
            SetCartInfos(new ItemInfo()
            {
                ItemNo = Guid.NewGuid(),
                ItemName = DateTime.UtcNow.Ticks.ToString()
            });
            return RedirectToAction("Cart", "Data");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCart()
        {
            string message = string.Empty;
            var cartInfos = GetCartInfos(ref message);

            if (cartInfos != null && cartInfos.Count()>0)
            {
                _context.Session.Remove(key:_sessionKeyCartName);
            }
            return RedirectToAction("Cart", "Data");
        }

        public IActionResult Cart()
        {

            string message = string.Empty;
            var cartInfos = GetCartInfos(ref message);

            ViewData["Message"] = message;
            return View(cartInfos);
        }
    }
    


}
