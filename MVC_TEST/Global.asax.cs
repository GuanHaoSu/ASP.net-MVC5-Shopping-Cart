using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using MVC_TEST.Models;
namespace MVC_TEST
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        //string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        //string roles = string.Empty;

                        //using (DbTESTEntities1 entities = new DbTESTEntities1())
                        //{
                        //    Account user = entities.Account.SingleOrDefault(u => u.name == username);

                        //    roles = user.roles;
                        //}
                        //let us extract the roles from our own custom cookie

                        FormsIdentity id = HttpContext.Current.User.Identity as FormsIdentity;
                        FormsAuthenticationTicket ticket = id.Ticket;
                        string roles = ticket.UserData;
                        string username = ticket.Name;
                        //Let us set the Pricipal with our user specific details
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                          new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));
                    }
                    catch (Exception)
                    {
                        //somehting went wrong
                    }
                }
            }
        } 
    }

}
