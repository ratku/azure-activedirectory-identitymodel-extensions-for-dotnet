using System.Web.Mvc;

namespace S2SMiddleTier.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Address  = WebApiApplication.Address;
            ViewBag.Endpoint = WebApiApplication.Endpoint;
            ViewBag.ClientId = WebApiApplication.ClientId;
            ViewBag.SiteName = WebApiApplication.SiteName;
            ViewBag.Title = "S2SMiddleTier";

            return View();
        }
    }
}
