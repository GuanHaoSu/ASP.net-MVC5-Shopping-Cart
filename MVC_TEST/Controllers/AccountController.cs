using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MVC_TEST.Models;
using System.Web.Security;
using System.Net.Mail;
using System.Data.Entity;
namespace MVC_TEST.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private DbTESTEntities1 db = new DbTESTEntities1();
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (DbTESTEntities1 entities = new DbTESTEntities1())
                {
                    string username = model.name;
                    string password = model.password;

                    // Now if our password was enctypted or hashed we would have done the
                    // same operation on the user entered password here, But for now
                    // since the password is in plain text lets just authenticate directly

                    Account userValid = entities.Account.SingleOrDefault(user => user.name == username && user.password == password);

                    // User found in the database
                    if (userValid!=null)
                    {
                        string userdata = userValid.roles;
                        string formsCookieStr = string.Empty;
                        HttpContext currentContext = System.Web.HttpContext.Current;
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(0,
                                username,
                                DateTime.Now,
                                DateTime.Now.AddMinutes(30),
                                false,
                                userdata,
                                FormsAuthentication.FormsCookiePath);
                        formsCookieStr = FormsAuthentication.Encrypt(ticket);
                        HttpCookie FormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, formsCookieStr);
                        currentContext.Response.Cookies.Add(FormsCookie);
                        //FormsAuthentication.SetAuthCookie(username, false);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }
            }
         

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult CreateAccountVerify(string name, string password, string phone, string email, string address)
        {
            string g;
            g = Guid.NewGuid().ToString();
            AccountVerify userValid = db.AccountVerify.SingleOrDefault(user => user.name == name);

            //Account accountCheck = db.Account.SingleOrDefault(user => user.name==name);

            var hadAccount = db.Account
                                .Any(f => f.name == name);

            string verifyUrl =Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port)+
                            "/Account/Verify?guidString=" + g + "@" + name;
            string msg = "請點擊連結啟動帳號 " + verifyUrl;
            if (hadAccount != true) { 
                    try
                    {
                        AccountVerify data = new AccountVerify
                        {
                            name = name,
                            password = password,
                            phone = phone,
                            email = email,
                            roles = "user",
                            address= address,
                            guid = g
                        };
                        db.AccountVerify.Add(data);
                        db.SaveChanges();
                        send_email(msg, "帳號注冊認證", email);

                        return Json("已寄送認證通知信", JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        return Json("認證通知寄送失敗", JsonRequestBehavior.AllowGet);
                    }
            }
            else 
            {
                return Json("已有此帳號名稱", JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        public ActionResult Verify(string guidString)
        {
            string msg = "";
            if(guidString==null)
            {
                msg = "無認證字串";
            }
            else if (guidString != null)
            { 
            string[] strs = guidString.Split('@');
            string guid = strs[0];
            string name = strs[1];
   
            var query = (from p in db.AccountVerify
                         where p.guid == guid && p.name == name
                         select p).FirstOrDefault();

            try
            {
                Account data = new Account
                {
                    name = query.name,
                    password = query.password,
                    phone = query.phone,
                    email = query.email,
                    address =query.address,
                    roles = query.roles
                };
                db.Account.Add(data);
                db.SaveChanges();
                AccountVerify account = db.AccountVerify.Find(query.id);
                db.AccountVerify.Remove(account);
                db.SaveChanges();
                msg = "帳號已啟動";
                //return Json("帳號已啟動", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                msg = "帳號啟動失敗";
                //return Json("帳號啟動失敗", JsonRequestBehavior.AllowGet);
            }
            }
            ViewData["msg"] = msg;
            return View();
        }
        public void send_email(string msg, string mysubject, string address)
        {
            MailMessage message = new MailMessage("", address);//MailMessage(寄信者, 收信者)
            message.IsBodyHtml = true;
            message.BodyEncoding = System.Text.Encoding.UTF8;//E-mail編碼
            message.Subject = mysubject;//E-mail主旨
            message.Body = msg;//E-mail內容

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);//設定E-mail Server和port
            smtpClient.Credentials = new System.Net.NetworkCredential("", "");
            smtpClient.EnableSsl = true;
            try { 
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage()
        {
            //ViewBag.StatusMessage =
            //    message == ManageMessageId.ChangePasswordSuccess ? "您的密碼已變更。"
            //    : message == ManageMessageId.SetPasswordSuccess ? "已設定您的密碼。"
            //    : message == ManageMessageId.RemoveLoginSuccess ? "已移除外部登入。"
            //    : message == ManageMessageId.Error ? "發生錯誤。"
            //    : "";
            //ViewBag.HasLocalPassword = HasPassword();
            //ViewBag.ReturnUrl = Url.Action("Manage");
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Request.Cookies[cookieName];
            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            string UserName = authTicket.Name;

            var Account = from a in db.Account
                          where a.name == UserName
                          select a;
            ViewData["id"] = Account.FirstOrDefault().id;
            ViewData["name"] = Account.FirstOrDefault().name;
            //ViewData["password"] = Account.FirstOrDefault().password;
            ViewData["phone"] = Account.FirstOrDefault().phone;
            ViewData["email"] = Account.FirstOrDefault().email;
            ViewData["address"] = Account.FirstOrDefault().address;
            return View();
        }
        [HttpPost]
        public JsonResult Update(int id, string name, string OldPassword,string password, string phone, string email, string address)
        {
            Account account = db.Account.Find(id);
            if (OldPassword != "" && password != "")
            {
                try {
                        Account userValid = db.Account.SingleOrDefault(user =>user.name==name && user.password == OldPassword);
                        if (userValid!=null)
                        {
                            account.name = name;
                            account.password = password;
                            account.phone = phone;
                            account.email = email;
                            account.address = address;
                            db.Entry(account).State = EntityState.Modified;
                            db.SaveChanges();
                            return Json("更新成功", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("舊密碼輸入錯誤", JsonRequestBehavior.AllowGet);
                        }
                        
                     }
                catch
                {
                    return Json("舊密碼輸入錯誤", JsonRequestBehavior.AllowGet);
                }
            }
            else if (OldPassword == "" && password == "")
            {
                account.name = name;
                account.phone = phone;
                account.email = email;
                account.address = address;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return Json("更新成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("請輸入新舊密碼以更改密碼", JsonRequestBehavior.AllowGet);
            }

            
        }
        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 要求重新導向至外部登入提供者
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // 從外部登入提供者處取得使用者資訊
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            //AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helper
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}