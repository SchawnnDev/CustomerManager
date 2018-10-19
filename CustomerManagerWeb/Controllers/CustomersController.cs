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

    }
}