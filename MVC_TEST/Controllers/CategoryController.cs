using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_TEST.Models;
namespace MVC_TEST.Controllers
{
    public class CategoryController : Controller
    {
        private DbTESTEntities1 db = new DbTESTEntities1();
        //
        // GET: /Category/
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            var category = (from c in db.Category
                           select c);
            return View(category);
        }
        [Authorize(Roles = "admin")]
        public JsonResult GetCategorys()
        {
            var category = (from c in db.Category
                            select c);
            return Json(category, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public JsonResult Create(string name)
        {

            try
            {
                Category data = new Category
                {
                    categorys = name,
                };
                db.Category.Add(data);
                db.SaveChanges();
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("新增失敗", JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public JsonResult DeleteCategory(int id)
        {

            Category category = db.Category.Find(id);
            if (category == null)
            {
                return Json("no Account", JsonRequestBehavior.AllowGet);
            }
            try
            {

                db.Category.Remove(category);
                db.SaveChanges();
                return Json("刪除成功", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("刪除失敗", JsonRequestBehavior.AllowGet);
            }
        }
      
    }
}
