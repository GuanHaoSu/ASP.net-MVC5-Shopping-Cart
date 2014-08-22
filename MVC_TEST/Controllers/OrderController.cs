using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_TEST.Models;
using System.Web.Security;
using System.Data.Entity;
namespace MVC_TEST.Controllers
{
    public class OrderController : Controller
    {
        private DbTESTEntities1 db = new DbTESTEntities1();
        // GET: /Order/
        public ActionResult Index()
        {
            return View();
        }
        //
        // GET: /Order/OrderPreview?products=
        [Authorize(Roles = "admin,user")]
        public ActionResult OrderPreview(string pstring)
        {
            string[] strs = pstring.Split(';');

            return View();
        }
        //
        // GET: /Order/Details/5
        [Authorize(Roles = "admin,user")]
        public ActionResult OrderList()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public JsonResult GetOrderList()
        {
            var orders = (from order in db.Orders
                          orderby order.Date descending
                          select new { order.id, order.Name, order.Email, order.Phone, order.Address, order.SumPrice, order.Date })
                         .ToList()
                         .Select(o => new { 
                                id = o.id,
                                Name = o.Name,
                                Email =o.Email,
                                Phone =o.Phone,
                                Address = o.Address,
                                SumPrice =o.SumPrice,
                                Date = o.Date.ToString() });

            return Json(orders, JsonRequestBehavior.AllowGet);
        }
         [Authorize(Roles = "admin,user")]
        public JsonResult GetOrderListDetail(int id)
        {
            var orders = from order in db.Orders
                         join detail in db.OrderDetail
                         on order.id equals detail.OrderID
                         where order.id == id
                         orderby order.id
                         select new { order.id, detail.ProductName, detail.Count,detail.UnitSumPrice };

            return Json(orders, JsonRequestBehavior.AllowGet);
        }
         [Authorize(Roles = "admin")]
         public JsonResult SearchOrderByName(string name)
         {
             var orders = (from order in db.Orders
                          where order.Name.Contains(name)
                          orderby order.Date descending
                          select new { order.id, order.Name, order.Email, order.Phone,order.Address, order.SumPrice, order.Date })
                         .ToList()
                         .Select(o => new { 
                                id = o.id,
                                Name = o.Name,
                                Email =o.Email,
                                Phone =o.Phone,
                                Address = o.Address,
                                SumPrice =o.SumPrice,
                                Date = o.Date.ToString() });
             return Json(orders, JsonRequestBehavior.AllowGet);
         }
 
         [HttpPost]
         public JsonResult DeleteOrder(int id)
         {

             Orders order = db.Orders.Find(id);
             if (order == null)
             {
                 return Json("no Account", JsonRequestBehavior.AllowGet);
             }
             try
             {

                 db.Orders.Remove(order);
                 db.SaveChanges();
                 db.OrderDetail.Where(x => x.OrderID == id)
                        .ToList().ForEach(s => db.OrderDetail.Remove(s));
                 db.SaveChanges();
                 return Json("刪除成功", JsonRequestBehavior.AllowGet);
             }
             catch
             {
                 return Json("刪除失敗", JsonRequestBehavior.AllowGet);
             }
         }
        //
        // GET: /Order/Create
         [HttpPost]
        [Authorize(Roles = "admin,user")]
         public JsonResult Create(List<String> products)
        {
            int TotalPrice = 0;
            bool OutofStock = false;
            for (int i = 0; i < products.Count; i++)
            {
                string[] strP = products[i].Split(';');
                int Pid = int.Parse(strP[0]);
                int Pcount = int.Parse(strP[1]);
                var ProductQuery = (from p in db.Product
                             where p.ProductID == Pid
                             select new {p.Price,p.Count}).FirstOrDefault();
                if (ProductQuery.Count < Pcount)
                {
                    OutofStock = true;
                }
                else if (ProductQuery.Count >= Pcount)
                {
                    TotalPrice += int.Parse(ProductQuery.Price)* Pcount;
                }
            }

            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Request.Cookies[cookieName];
            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            string UserName = authTicket.Name;

          
            var AccountQuery = (from p in db.Account
                         where p.name == UserName
                         select p).FirstOrDefault();
            try
            {
                if (OutofStock == false)
                {
                    Orders data = new Orders
                    {

                        Name = AccountQuery.name,
                        Email = AccountQuery.email,
                        Address = AccountQuery.address,
                        Phone = AccountQuery.phone,
                        SumPrice = TotalPrice,
                        Date = DateTime.Now,
                        CheckOut = 0
                    };
                    db.Orders.Add(data);
                    db.SaveChanges();
                    int Aid = data.id;

                    for (int i = 0; i < products.Count; i++)
                    {
                        string[] strP = products[i].Split(';');
                        int Pid = int.Parse(strP[0]);
                        int Pcount = int.Parse(strP[1]);
                        var ProductQuery = (from p in db.Product
                                            where p.ProductID == Pid
                                            select p).FirstOrDefault();
                        int unitPrice = int.Parse(ProductQuery.Price) * Pcount;
                        OrderDetail data2 = new OrderDetail
                        {
                            OrderID = Aid,
                            ProductID = Pid,
                            ProductName = ProductQuery.Name,
                            Count = Pcount,
                            UnitSumPrice = unitPrice

                        };
                        db.OrderDetail.Add(data2);
                        db.SaveChanges();

                        Product Product = db.Product.Find(Pid);
                        Product.Count = ProductQuery.Count - Pcount;
                        db.Entry(Product).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (OutofStock == true)
                {
                    return Json("CountNotEnough", JsonRequestBehavior.AllowGet);
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
            

        }

        
         //POST: /Order/Create
         public ActionResult Create()
        {
                return View();
            
        }

         public ActionResult CostumerOrderList()
         {
             string cookieName = FormsAuthentication.FormsCookieName;
             HttpCookie authCookie = Request.Cookies[cookieName];
             FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
             string UserName = authTicket.Name;
             //Orders orderObj = new Orders();

             var orders = (from order in db.Orders
                           where order.Name == UserName
                           orderby order.Date descending
                           select new { order.id, order.Name, order.Email, order.Phone, order.Address, order.SumPrice, order.Date })
                        .ToList()
                        .Select(o => new Orders
                        {
                            id = o.id,
                            Name = o.Name,
                            Email = o.Email,
                            Phone = o.Phone,
                            Address = o.Address,
                            SumPrice = o.SumPrice,
                            Date = o.Date
                        });

             return View(orders);

         }
      
    }
}
