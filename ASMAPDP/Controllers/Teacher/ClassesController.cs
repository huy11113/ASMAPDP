using ASMAPDP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASMAPDP.Controllers.Teacher
{
    [Route("teacher/classes")]
    public class ClassesController : Controller
    {
        private string classFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "classes.csv");

        // Hiển thị danh sách lớp học
        public IActionResult Index()
        {
            var teacherUsername = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(teacherUsername) || HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToAction("Login", "Auth");
            }

            var classes = ReadClassesFromCsv();
            var teacherClasses = classes.Where(c => c.TeacherUsername == teacherUsername).ToList();
            return View("~/Views/Teacher/TeacherClass.cshtml", teacherClasses);
        }

        [HttpGet]
        [Route("teacher/classes/add")]
        public IActionResult Add()
        {
            if (HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToAction("Login", "Auth");
            }
            return View("~/Views/Teacher/Add.cshtml");
        }

        [HttpPost]
        [Route("teacher/classes/add")]
        public IActionResult Add(Class model)
        {
            // Bỏ qua validation cho TeacherUsername
            ModelState.Remove("TeacherUsername");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Validation errors: " + string.Join(", ", errors));
                return View("~/Views/Teacher/Add.cshtml", model);
            }

            var teacherUsername = HttpContext.Session.GetString("Username");
            Console.WriteLine($"Teacher Username: {teacherUsername}");

            var classes = ReadClassesFromCsv();
            Console.WriteLine($"Number of classes before adding: {classes.Count}");

            model.Id = classes.Count + 1;
            model.TeacherUsername = teacherUsername;
            classes.Add(model);

            WriteClassesToCsv(classes);
            Console.WriteLine($"Number of classes after adding: {classes.Count}");

            return RedirectToAction("Index");
        }

        // Xóa lớp học
        [HttpPost]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var teacherUsername = HttpContext.Session.GetString("Username");
            var classes = ReadClassesFromCsv();

            var classToRemove = classes.FirstOrDefault(c => c.Id == id && c.TeacherUsername == teacherUsername);
            if (classToRemove != null)
            {
                classes.Remove(classToRemove);
                WriteClassesToCsv(classes);
            }
            return RedirectToAction("Index");
        }

        // Đọc danh sách lớp từ CSV
        private List<Class> ReadClassesFromCsv()
        {
            List<Class> classes = new List<Class>();
            if (!System.IO.File.Exists(classFilePath))
            {
                // Tạo file với dòng tiêu đề nếu chưa tồn tại
                System.IO.File.WriteAllLines(classFilePath, new[] { "Id,ClassName,TeacherUsername,Schedule" });
                return classes;
            }

            var lines = System.IO.File.ReadAllLines(classFilePath);
            foreach (var line in lines.Skip(1)) // Bỏ qua tiêu đề
            {
                var values = line.Split(',');
                classes.Add(new Class
                {
                    Id = int.Parse(values[0]),
                    ClassName = values[1],
                    TeacherUsername = values[2],
                    Schedule = values[3]
                });
            }
            return classes;
        }

        // Ghi danh sách lớp vào CSV
        private void WriteClassesToCsv(List<Class> classes)
        {
            try
            {
                var lines = new List<string> { "Id,ClassName,TeacherUsername,Schedule" };
                lines.AddRange(classes.Select(c => $"{c.Id},{c.ClassName},{c.TeacherUsername},{c.Schedule}"));
                System.IO.File.WriteAllLines(classFilePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to classes.csv: " + ex.Message);
                throw;
            }
        }
    }
}
