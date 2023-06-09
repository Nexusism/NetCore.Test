﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NetCore.Data.ViewModels;
using NetCore.Services.interfaces;
using NetCore.Services.Svcs;
using NetCore.Web.Models;
using System.Security.Claims;

namespace NetCore.Web.Controllers
{
    

    [Authorize(Roles = "AssociateUser, GeneralUser ,SuperUser, SystemUser")]
    public class EUserController : Controller
    {

        // 의존성 주입 - 생성자 주입(닷넷코어 기본제공)
        private readonly ILogger<EUserController> _logger;
        private IUser _user;
        private IPasswordHasher _hasher;
        private HttpContext _context;

        public EUserController(IHttpContextAccessor accessor, IPasswordHasher hasher, IUser user, ILogger<EUserController> logger)
        {
            _context = accessor.HttpContext;
            _hasher = hasher;
            _user = user;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into EUserController");
        }

        #region private methods
        /// <summary>
        /// 로컬URL인지 외부URL인지 체크
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(EUserController.Index), "EUser");
            }
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            _logger.LogInformation("로그인화면 진입");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            TempData["Message"] = "로그아웃 되었습니다.";
            return View();
        }

        [HttpPost("/EUser/Login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        // Services => Web
        // Services 
        public async Task<IActionResult> LoginAsync(LoginInfo login, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            //Console.WriteLine("LoginAsync 진입");
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                //await Console.Out.WriteLineAsync("Match 결과 " + _user.MatchTheUserInfo(login));
                // 뷰모델
                // 서비스 개념
                //if (login.UserId.Equals(userId) && login.Password.Equals(password))
                //if (_hasher.MatchTheUserInfo(login.UserId, login.Password))
                if (_user.MatchTheUserInfo(login))
                {
                    
                    //신원보증과 승인권한
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();
                    string userDataInfo = userTopRole.UserRole.RoleName + "|" +
                                          userTopRole.UserRole.RolePriority.ToString() + "|" +
                                          userInfo.UserName + "|" +
                                          userInfo.UserEmail;

                    // _context.User.Identity.Name = 사용자 아이디

                    var identity = new ClaimsIdentity(claims: new[]
                    {
                        new Claim(type:ClaimTypes.Name,
                                  value: userInfo.UserId),
                        new Claim(type:ClaimTypes.Role,
                                  value: userTopRole.RoleId),
                        new Claim(type:ClaimTypes.UserData,
                                    value: userDataInfo)
                    },authenticationType:CookieAuthenticationDefaults.AuthenticationScheme);
                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                                               principal: new ClaimsPrincipal(identity: identity),
                                               properties: new AuthenticationProperties()
                                               {
                                                   IsPersistent = login.RememberMe,
                                                   ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30),
                                               });
                
                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다.";

                    //return RedirectToAction("Index", "EUser");
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    message = "로그인되지 않았습니다.";
                }
            }
            else
            {
                message = "로그인 정보를 올바르게 입력하세요.";

            }

            ModelState.AddModelError(string.Empty, message);
            return View("Login", login);
        }

        [HttpGet]
        [Authorize(Roles = "SystemUser")]
        public IActionResult Register(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterInfo register, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                // 가입 서비스
                if(_user.RegisterUser(register) > 0)
                {
                    TempData["Message"] = "회원가입이 성공적으로 이루어졌습니다.";
                    return RedirectToAction("Login", "EUser");
                }
                else
                {
                    message = "사용자등록이 완료되지 않았습니다.";

                }

            }
            else
            {
                message = "가입정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(register);
        }
        [HttpGet]
        public IActionResult UpdateInfo()
        {
            UserInfo user = _user.GetUserInfoForUpdate(_context.User.Identity.Name); // 서비스
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInfo(UserInfo user)
        {
            Console.WriteLine("userName = " + user.UserName);
            Console.WriteLine("userId = " + user.UserId);
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                // 변경대상 값 비교 서비스
                if (_user.CompareInfo(user))
                {
                    message = "변경내용이 없습니다.";
                    ModelState.AddModelError(string.Empty, message);
                    return View(user);
                }

                // 정보 수정 서비스
                if (_user.UpdateUser(user) > 0)
                {
                    TempData["Message"] = "수정되었습니다.";
                    return RedirectToAction("UpdateInfo", "EUser");
                }
                else
                {
                    message = "사용자 정보가 일치하지 않습니다.";
                }
            }
            else
            {
                message = "변경사항이 없습니다.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(user);
        }

        [HttpPost("/EUser/Withdrawn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithDrawnAsync(WithdrawnInfo withdrawn) {

            string message = string.Empty;
            if(ModelState.IsValid)
            {
                // 탈퇴 서비스
                if(_user.WithdrawnUser(withdrawn) > 0)
                {
                    TempData["Message"] = "사용자 탈퇴가 완료되었습니다.";

                    await _context.SignOutAsync(scheme:CookieAuthenticationDefaults.AuthenticationScheme);

                    return RedirectToAction("Login", "EUser");
                }
                else
                {
                    message = "사용자 탈퇴에 실패하였습니다. 비밀번호를 확인하세요.";
                }

            }
            else
            {
                message = "사용자 탈퇴에 실패하였습니다. 비밀번호를 확인하세요.";
            }
            ViewData["Message"] = message;
            return View("Index", withdrawn);
        }

        [HttpGet("/EUser/LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await _context.SignOutAsync(scheme:CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "로그아웃 되었습니다. <br />";
            //return RedirectToAction("LogOut", "EUser");
            return RedirectToAction("Login", "EUser");
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            StringValues paramReturnUrl;
            bool exists = _context.Request.Query.TryGetValue("returnUrl", out paramReturnUrl);
            paramReturnUrl = exists ? _context.Request.Host.Value + paramReturnUrl[0] : string.Empty;

            ViewData["Message"] = $"{paramReturnUrl} 에 대한 권한이 없습니다. <br />" + 
                                   "관리자에게 문의하세요.";
            return View();
        }

    }
}
