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

    }
}