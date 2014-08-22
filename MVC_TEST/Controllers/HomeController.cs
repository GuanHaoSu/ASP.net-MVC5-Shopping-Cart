using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_TEST.Models;
namespace MVC_TEST.Controllers
{
    public class HomeController : Controller
    {
        private DbTESTEntities1 db = new DbTESTEntities1();

        public ActionResult Index()
        {
            var product = (from p in db.Product
                           select p);

            var categorys = (from category in db.Category
                             select category).ToList();

             ViewData["category"] = categorys;
            //return product.ToList();
            return View(product.ToList().Take(10));
        }
        public JsonResult GetProductCount(int id)
        {
            var products = from product in db.Product
                           where  product.ProductID ==id
                           select new { product.Count};
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductByCategorys(string Category)
        {
            var product = (from p in db.Product
                           where p.Category == Category
                           select p);
            return Json(product, JsonRequestBehavior.AllowGet);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Cart()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}