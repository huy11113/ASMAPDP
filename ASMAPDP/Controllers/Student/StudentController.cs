using Microsoft.AspNetCore.Mvc;

namespace ASMAPDP.Controllers.Student
{
    public class StudentController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
