using System;
using System.Collections.Generic;
using CustomerManagement.Data;
using System.Web.Mvc;

namespace CustomerManagerWeb.Controllers
{
    public class CustomersController : Controller
    {

        // GET: Customer
        public ActionResult Index()
        {
            DbManager.DataSource = @"GRIEVOUS\HISTORIAN";
            DbManager.DatabaseName = "CustomerManager";
            var customers = DbManager.LoadData();
            return View(customers);
        }

        [HttpPost, ValidateHeaderAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            DbManager.DeleteShippingAddress(id); // Should 
            return Json("[{\"result\":\"success\"}]");
        }

        [HttpGet]
        public ActionResult Manage(int id)
        {
            Customer customer = null;

            if (id < 0)
                return View("Error");

            if (id != 0)
                customer = DbManager.GetCustomer(id);

            if (customer == null) customer = new Customer();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(FormCollection collection)
        {
            var id = int.Parse(collection["id"]);
            var firstName = collection["firstName"];
            var name = collection["name"];
            var dateOfBirthStr = collection["dateOfBirth"];
            var phoneNumber = collection["phoneNumber"];
            var email = collection["email"];

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(name) || !DateTime.TryParse(dateOfBirthStr, out var dateOfBirth) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(email)) return View("Error");

            var customer = new Customer(firstName, name, dateOfBirth, phoneNumber, email)
            {
                Id = id
            };

            if (id == 0)
                DbManager.SaveCustomersToDB(new List<Customer>() { customer });
            else
                DbManager.UpdateCustomer(customer);

            return RedirectToAction("Index", "Customers");

        }

    }
}