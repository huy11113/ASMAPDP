using Microsoft.AspNetCore.Mvc;

namespace ASMAPDP.Controllers
{
    public class AuthControllers : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
