using Microsoft.AspNetCore.Mvc;

namespace ASMAPDP.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
