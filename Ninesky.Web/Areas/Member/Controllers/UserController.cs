using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninesky.Common;
using System.Drawing;
using Ninesky.Web.Areas.Member.Models;
using Ninesky.Models;
using Ninesky.IBLL;
using Ninesky.BLL;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace Ninesky.Web.Areas.Member.Controllers
{
    [Authorize]
    public class UserController : Controller
    {


        #region 属性
        private IAuthenticationManager AuthenticationManager { get { return HttpContext.GetOwinContext().Authentication; } }
        private InterfaceUserService userService;

        #endregion

        public UserController()
        {
            userService = new UserService();
        }

        //
        // GET: /Member/User/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult VerificationCode()
        {
            string verificationCode = Security.CreateVerificationText(6);
            Bitmap _img = Security.CreateVerificationImage(verificationCode, 160, 30);
            _img.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            TempData["VerificationCode"] = verificationCode.ToUpper();
            return null;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel register)
        {
            if (TempData["VerificationCode"] == null || TempData["VerificationCode"].ToString() != register.VerificationCode.ToUpper())
            {
                ModelState.AddModelError("VerificationCode", "验证码不正确");
                return View(register);
            }
            if (ModelState.IsValid)
            {

                if (userService.Exist(register.UserName)) ModelState.AddModelError("UserName", "用户名已存在");
                else
                {
                    User _user = new User()
                    {
                        UserName = register.UserName,
                        //默认用户组代码写这里
                        DisplayName = register.DisplayName,
                        Password = Security.Sha256(register.Password),
                        //邮箱验证与邮箱唯一性问题
                        Email = register.Email,
                        //用户状态问题
                        Status = 0,
                        RegistrationTime = System.DateTime.Now,
                        LoginTime = System.DateTime.Now,
                        LoginIP = Request.UserHostAddress
                    };
                    _user = userService.Add(_user);
                    if (_user.UserID > 0)
                    {
                        var _identity = userService.CreateIdentity(_user, DefaultAuthenticationTypes.ApplicationCookie);
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        AuthenticationManager.SignIn(_identity);
                        return RedirectToAction("Index", "Home");
                    }
                    else { ModelState.AddModelError("", "注册失败！"); }
                }
            }
            return View(register);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="returnUrl">返回Url</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var _user = userService.Find(loginViewModel.UserName);
                if (_user == null) ModelState.AddModelError("UserName", "用户名不存在");
                else if (_user.Password == Common.Security.Sha256(loginViewModel.Password))
                {
                    _user.LoginTime = System.DateTime.Now;
                    _user.LoginIP = Request.UserHostAddress;
                    userService.Update(_user);
                    var _identity = userService.CreateIdentity(_user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = loginViewModel.RememberMe }, _identity);
                    return RedirectToAction("Index", "Home");
                }
                else ModelState.AddModelError("Password", "密码错误");
            }
            return View();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect(Url.Content("~/"));
        }

        public ActionResult Menu()
        {
            return View();
        }

        /// <summary>
        /// 显示资料
        /// </summary>
        /// <returns></returns>
        public ActionResult Details()
        {
            return View(userService.Find(User.Identity.Name));
        }

        /// <summary>
        /// 修改资料
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Modify()
        {

            var _user = userService.Find(User.Identity.Name);
            if (_user == null) ModelState.AddModelError("", "用户不存在");
            else
            {
                if (TryUpdateModel(_user, new string[] { "DisplayName", "Email" }))
                {
                    if (ModelState.IsValid)
                    {
                        if (userService.Update(_user)) ModelState.AddModelError("", "修改成功！");
                        else ModelState.AddModelError("", "无需要修改的资料");
                    }
                }
                else ModelState.AddModelError("", "更新模型数据失败");
            }
            return View("Details", _user);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel passwordViewModel)
        {
            if (ModelState.IsValid)
            {
                var _user = userService.Find(User.Identity.Name);
                if (_user.Password == Common.Security.Sha256(passwordViewModel.OriginalPassword))
                {
                    _user.Password = Common.Security.Sha256(passwordViewModel.Password);
                    if (userService.Update(_user)) ModelState.AddModelError("", "修改密码成功");
                    else ModelState.AddModelError("", "修改密码失败");
                }
                else ModelState.AddModelError("", "原密码错误");
            }
            return View(passwordViewModel);
        }
    }
}