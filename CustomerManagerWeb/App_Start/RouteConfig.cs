using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CustomerManagerWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { area = "Customers", controller = "Customers", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShippingAddresses",
                "{controller}/{action}/{id}/{addressId}",
                new { controller = "ShippingAddresses", action = "Index", addressId = UrlParameter.Optional }
            );

        }
    }
}
