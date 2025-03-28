using ASMAPDP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASMAPDP.Controllers.Teacher
{
    [Route("teacher/students")]
    public class TeacherStudentListController : Controller
    {
        private string studentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.csv");

        // Hiển thị danh sách sinh viên
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            var teacherUsername = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(teacherUsername) || HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToAction("Login", "Auth");
            }

            // Đọc danh sách sinh viên
            var students = ReadStudentsFromCsv();
            var studentList = students.Where(s => s.Role == UserRole.Student).ToList();

            return View("~/Views/Teacher/TeacherStudentList.cshtml", studentList);
        }

        // Đọc danh sách sinh viên từ CSV
        private List<User> ReadStudentsFromCsv()
        {
            List<User> users = new List<User>();
            if (!System.IO.File.Exists(studentFilePath))
            {
                System.IO.File.WriteAllLines(studentFilePath, new[] { "Id,FullName,Address,Username,Password,Role" });
                return users;
            }

            var lines = System.IO.File.ReadAllLines(studentFilePath);
            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(',');
                users.Add(new User
                {
                    Id = int.Parse(values[0]),
                    FullName = values[1],
                    Address = values[2],
                    Username = values[3],
                    Password = values[4],
                    Role = (UserRole)Enum.Parse(typeof(UserRole), values[5])
                });
            }
            return users;
        }
    }
}
