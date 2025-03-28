using Microsoft.AspNetCore.Mvc;

namespace ASMAPDP.Controllers.Teacher
{
    public class TeacherController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
