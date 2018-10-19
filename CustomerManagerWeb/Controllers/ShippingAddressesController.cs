using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomerManagement.Data;
using CustomerManager.Data;

namespace CustomerManagerWeb.Controllers
{
    public class ShippingAddressesController : Controller
    {
        // GET: ShippingAddresses
        public ActionResult Index(int id)
        {
            var shippingAddresses = DbManager.GetShippingAddresses(id);
            return View(shippingAddresses);
        }

        [HttpGet]
        public ActionResult Manage(int id, int addressId)
        {
            ShippingAddress address = null;

            if (addressId < 0 || id <= 0)
            {

                return View("Error");
            }

            if (addressId != 0)
            {
                address = DataManager.GetShippingAddress(DbManager.GetShippingAddresses(id), addressId);
            }

            if (address == null) address = new ShippingAddress(id);

            return View(address);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ShippingAddress address)
        {
            return View();
        }

        [HttpPost, ValidateHeaderAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            DbManager.DeleteShippingAddress(id); // Should 
            return Json("[{\"result\":\"success\"}]");
        }

    }
}