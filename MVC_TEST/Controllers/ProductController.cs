using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using MVC_TEST.Models;
using System.IO;
using Newtonsoft.Json;
using System.Text;
namespace MVC_TEST.Controllers
{
    public class ProductController : Controller
    {
        private DbTESTEntities1 db = new DbTESTEntities1();
        //
        // GET: /Product/
        public ActionResult Index()
        {
            return View();
        }
        // GET: /Product/ProductList
        public ActionResult ProductList()
        {
            var category = (from c in db.Category
                            select c).ToList();
            var selectList = new SelectList(category, "categorys", "categorys");
            ViewBag.CategorySelectList = selectList;
            return View();
        }
        //
        // GET: /Product/GetProductList
        public JsonResult GetProductList()
        {
            var products = from product in db.Product
                           orderby product.ProductID
                           select new { product.ProductID, product.Name, product.Image, product.Price, product.Count, product.Category };
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult CreateProduct(HttpPostedFileBase file,string name, string price, int count, string category)
        {
            string fileName = "";
            if (file != null) { 
                    if (file.ContentLength > 0)
                    {
                        fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/ProductImage"), fileName);
                        file.SaveAs(path);
                    }
            }
            try
            {
                Product data = new Product
                {
                    Name = name,
                    Image = fileName,
                    Price = price,
                    Count = count,
                    Category = category
                };
                db.Product.Add(data);
                db.SaveChanges();
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("新增失敗", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult UpdateProduct(HttpPostedFileBase file, int ProductID, string name, string price, int count, string category)
        {
            
            string fileName = "";
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var productImage = (from products in db.Product
                                  where products.ProductID == ProductID
                                           select new { products.Image }).FirstOrDefault();

                    string fullPath = Server.MapPath("~/ProductImage/" + productImage.Image);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/ProductImage"), fileName);
                    file.SaveAs(path);
                }
              
            }
            else 
            {
                var productImage = (from products in db.Product
                                    where products.ProductID == ProductID
                                    select new { products.Image }).FirstOrDefault();
                fileName = productImage.Image;
            }
            Product product = db.Product.Find(ProductID);
            try
            {

                    product.Name = name;
                    product.Image = fileName;
                    product.Price = price;
                    product.Count = count;
                    product.Category = category;
                    db.Entry(product).State = EntityState.Modified;
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
        public JsonResult DeleteProduct(int ProductID)
        {

            Product product = db.Product.Find(ProductID);
            if (product == null)
            {
                return Json("no product", JsonRequestBehavior.AllowGet);
            }
            try
            {
                var productImage = (from products in db.Product
                                    where products.ProductID == ProductID
                                    select new { products.Image }).FirstOrDefault();

                string fullPath = Server.MapPath("~/ProductImage/" + productImage.Image);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                db.Product.Remove(product);
                db.SaveChanges();
               
                return Json("刪除成功", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("刪除失敗", JsonRequestBehavior.AllowGet);
            }
        }
       
        //
        // GET: /Product/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }
        public class Polygon
        {
            public string Longitude { get; set; }
            public string Latitude { get; set; }
        }

        public class RootObject
        {
            public string City { get; set; }
            public List<Polygon> polygon { get; set; }
            public string Infor { get; set; }
            public string fillOpacity { get; set; }
            public string fillColor { get; set; }
            public string Label { get; set; }
            public string Toatal { get; set; }
        }
        public ActionResult GetJsonTEST()
        {
            List<Polygon> Polygons = new List<Polygon>();
            
            RootObject root = new RootObject();
            root.City = "taipei";
            Polygon data = new Polygon();
            data.Latitude = "121.70876503";
            data.Longitude = "25.173933029000068";
            Polygons.Add(data);
            data.Latitude = "123.70876503";
            data.Longitude = "27.173933029000068";
            Polygons.Add(data);
            root.polygon = Polygons;
            root.Infor = "taipei number";

            List<RootObject> list = new List<RootObject>();
            for (int i = 0; i < 50; i++)
            {
                list.Add(root);
            }

            string strJson = JsonConvert.SerializeObject(list, Formatting.Indented);


          //string result = JsonConvert.SerializeObject(sb.ToString(), Formatting.Indented);
            return Content(strJson.ToString(), "application/json");
        }
       
    }
}
