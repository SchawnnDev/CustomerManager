using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using CustomerManagement.Data;
using CustomerManager.Data;
using Microsoft.Ajax.Utilities;

namespace CustomerManagerWeb.Controllers
{
    public class ShippingAddressesController : Controller
    {
        // GET: ShippingAddresses
        public ActionResult Index(int id)
        {
            ViewData["id"] = id;
            var shippingAddresses = DbManager.GetShippingAddresses(id);
            return View(shippingAddresses);
        }

        [HttpGet]
        public ActionResult Manage(int id, int addressId)
        {
            ShippingAddress address = null;

            if (addressId < 0 || id <= 0)
                return View("Error");

            if (addressId != 0)
                address = DataManager.GetShippingAddress(DbManager.GetShippingAddresses(id), addressId);

            if (address == null) address = new ShippingAddress(id);

            return View(address);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(FormCollection collection)
        {
            var id = int.Parse(collection["id"]);
            var customerId = int.Parse(collection["customerId"]);
            var address = collection["address"];
            var postalCode = collection["postalCode"];

            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(postalCode)) return View("Error");

            ShippingAddress shippingAddress = new ShippingAddress(id, customerId, address, postalCode);

            if (id == 0)
                DbManager.SaveShippingAddressesToDB(new List<ShippingAddress>() { shippingAddress });
            else
                DbManager.UpdateShippingAddress(shippingAddress);

            return RedirectToAction("Index", "Customers");

        }

        [HttpPost, ValidateHeaderAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            DbManager.DeleteShippingAddress(id); // Should 
            return Json("[{\"result\":\"success\"}]");
        }

    }
}