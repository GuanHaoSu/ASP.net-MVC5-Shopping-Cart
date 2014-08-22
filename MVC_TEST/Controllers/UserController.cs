using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_TEST.Models;
using System.Web.Security;
namespace MVC_TEST.Controllers
{
    public class UserController : Controller
    {

        private DbTESTEntities1 db = new DbTESTEntities1();
        // GET: /User/
        
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View(db.Account.ToList());
        }
        [Authorize(Roles = "admin")]
        public ActionResult AccountList()
        {
           
           // return Json(db.Account.ToList(), JsonRequestBehavior.AllowGet);
            return View();
        }
         [Authorize(Roles = "admin")]
        public ActionResult GetAccountList()
        {

            return Json(db.Account.ToList(), JsonRequestBehavior.AllowGet);
        }

         public ActionResult GetCount()
         {
             var query = db.Account
                 .OrderBy(c => c.id);
             return Json(query.Count(), JsonRequestBehavior.AllowGet);
         }
        [HttpPost]
         public JsonResult GetPage(int? page, int pageSize)
         {
             //const int pageSize = 5;
             var users = GetPagedCustomers((page ?? 0) * pageSize, pageSize);
             return Json(users, JsonRequestBehavior.AllowGet);
         }
         public static List<Account> GetPagedCustomers(int skip, int take)
         {
             using (var context = new DbTESTEntities1())
             {
                 var query = context.Account
                  .OrderBy(c => c.id );
                 return query.Skip(skip).Take(take).ToList();
             }
         }
        [Authorize(Roles = "admin,user")]
        public ActionResult GetRole()
        {
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Request.Cookies[cookieName];
            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            string rolesArray = authTicket.UserData;

            return Json(rolesArray, JsonRequestBehavior.AllowGet);
        }

        // GET: /User/Details/5
        [Authorize(Roles = "admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        public ActionResult GetDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return Json(account, JsonRequestBehavior.AllowGet);
        }
        // GET: /User/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /User/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,name,password,phone,email")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Account.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }
        [HttpPost]
        //[Authorize(Roles = "admin")]
        public JsonResult CreateAccount(string name,string password, string phone, string email,string roles,string address)
        {
            Account userValid = db.Account.SingleOrDefault(user => user.name == name);
            if (userValid != null)
            {
                return Json("已有此帳號", JsonRequestBehavior.AllowGet);
            }
            try
            {
                Account data = new Account
                {
                    name = name,
                    password =password,
                    phone = phone,
                    email = email,
                    address = address,
                    roles = roles
                };
                db.Account.Add(data);
                db.SaveChanges();
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("新增失敗", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UpdateAccount(int id, string name, string password, string phone, string email, string roles, string address)
        {

            Account account = db.Account.Find(id);
            if (account == null)
            {
                return Json("no Account", JsonRequestBehavior.AllowGet);
            }
            try
            {
                    account.name = name;
                    account.password = password;
                    account.phone = phone;
                    account.email = email;
                    account.address = address;
                    account.roles = roles;
                    db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return Json("更新成功" + id, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("更新失敗", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DeleteAccount(int id, string name, string password, string phone, string email)
        {

            Account account = db.Account.Find(id);
            if (account == null)
            {
                return Json("no Account", JsonRequestBehavior.AllowGet);
            }
            try
            {
                db.Account.Remove(account);
                db.SaveChanges();
                return Json("刪除成功", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("刪除失敗", JsonRequestBehavior.AllowGet);
            }
        }
        // GET: /User/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: /User/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,name,password,phone,email")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: /User/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: /User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Account.Find(id);
            db.Account.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
