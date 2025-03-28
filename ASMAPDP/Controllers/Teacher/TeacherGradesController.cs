using ASMAPDP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASMAPDP.Controllers.Teacher
{
    [Route("teacher/grades")]
    public class GradesController : Controller
    {
        private string classFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "classes.csv");
        private string gradeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "grades.csv");
        private string studentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.csv");

        // Hiển thị danh sách lớp học để chọn lớp nhập điểm
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            var teacherUsername = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(teacherUsername) || HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToAction("Login", "Auth");
            }

            // Đọc danh sách lớp học của giáo viên
            var classes = ReadClassesFromCsv();
            var teacherClasses = classes.Where(c => c.TeacherUsername == teacherUsername).ToList();

            return View("~/Views/Teacher/GradesIndex.cshtml", teacherClasses);
        }

        // Hiển thị danh sách sinh viên trong lớp để nhập điểm
        [HttpGet]
        [Route("class/{classId}")]
        public IActionResult EnterGrades(int classId)
        {
            var teacherUsername = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(teacherUsername) || HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToAction("Login", "Auth");
            }

            // Đọc danh sách lớp học
            var classes = ReadClassesFromCsv();
            var classInfo = classes.FirstOrDefault(c => c.Id == classId && c.TeacherUsername == teacherUsername);
            if (classInfo == null)
            {
                return NotFound("Lớp học không tồn tại hoặc bạn không có quyền truy cập.");
            }

            // Đọc danh sách sinh viên
            var students = ReadStudentsFromCsv();
            var studentList = students.Where(s => s.Role == UserRole.Student).ToList();

            // Đọc danh sách điểm hiện tại
            var grades = ReadGradesFromCsv();
            var studentGrades = studentList.Select(student => new StudentGradeViewModel
            {
                StudentUsername = student.Username,
                FullName = student.FullName,
                Score = grades.FirstOrDefault(g => g.ClassId == classId && g.StudentUsername == student.Username)?.Score ?? 0
            }).ToList();

            ViewBag.ClassId = classId;
            ViewBag.ClassName = classInfo.ClassName;
            return View("~/Views/Teacher/EnterGrades.cshtml", studentGrades);
        }

        // Lưu điểm
        [HttpPost]
        [Route("save")]
        public IActionResult SaveGrades(int classId, Dictionary<string, double> scores)
        {
            var teacherUsername = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(teacherUsername) || HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToAction("Login", "Auth");
            }

            // Đọc danh sách điểm hiện tại
            var grades = ReadGradesFromCsv();

            // Xóa các điểm cũ của lớp này
            grades.RemoveAll(g => g.ClassId == classId);

            // Thêm điểm mới
            foreach (var score in scores)
            {
                grades.Add(new Grade
                {
                    ClassId = classId,
                    StudentUsername = score.Key,
                    Score = score.Value,
                    TeacherUsername = teacherUsername
                });
            }

            // Ghi lại vào file
            WriteGradesToCsv(grades);

            return RedirectToAction("Index", "Classes");
        }

        // Đọc danh sách lớp từ CSV
        private List<Class> ReadClassesFromCsv()
        {
            List<Class> classes = new List<Class>();
            if (!System.IO.File.Exists(classFilePath))
            {
                System.IO.File.WriteAllLines(classFilePath, new[] { "Id,ClassName,TeacherUsername,Schedule" });
                return classes;
            }

            var lines = System.IO.File.ReadAllLines(classFilePath);
            foreach (var line in lines.Skip(1))
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

        // Đọc danh sách điểm từ CSV
        private List<Grade> ReadGradesFromCsv()
        {
            List<Grade> grades = new List<Grade>();
            if (!System.IO.File.Exists(gradeFilePath))
            {
                System.IO.File.WriteAllLines(gradeFilePath, new[] { "ClassId,StudentUsername,Score,TeacherUsername" });
                return grades;
            }

            var lines = System.IO.File.ReadAllLines(gradeFilePath);
            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(',');
                grades.Add(new Grade
                {
                    ClassId = int.Parse(values[0]),
                    StudentUsername = values[1],
                    Score = double.Parse(values[2]),
                    TeacherUsername = values[3]
                });
            }
            return grades;
        }

        // Ghi danh sách điểm vào CSV
        private void WriteGradesToCsv(List<Grade> grades)
        {
            try
            {
                var lines = new List<string> { "ClassId,StudentUsername,Score,TeacherUsername" };
                lines.AddRange(grades.Select(g => $"{g.ClassId},{g.StudentUsername},{g.Score},{g.TeacherUsername}"));
                System.IO.File.WriteAllLines(gradeFilePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to grades.csv: " + ex.Message);
                throw;
            }
        }
    }
}
