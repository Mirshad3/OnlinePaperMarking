using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlinePapermarking.Controllers
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        private bool AuthorizeUser(AuthorizationContext filterContext)
        {
            bool isAuthorized = false;

            if (filterContext.RequestContext.HttpContext != null)
            {
                var context = filterContext.RequestContext.HttpContext;

                if (context.Session["LoginId"] != null)
                    isAuthorized = true;
            }
            return isAuthorized;
        }

        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    if (filterContext == null)
        //        throw new ArgumentNullException("filterContext");

        //    if (AuthorizeUser(filterContext))
        //        return;

        //    base.OnAuthorization(filterContext);
        //}

        public override void OnAuthorization(AuthorizationContext filterContext) {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (AuthorizeUser(filterContext))
                return;
            else {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
           }
        }
    }
}