using ASMAPDP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASMAPDP.Controllers
{
    public class AuthController : Controller
    {
        private string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.csv");


        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                List<User> users = ReadUsersFromCsv();
                if (users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("", "Username đã tồn tại!");
                    return View(model);
                }

                model.Id = users.Count + 1;
                users.Add(model);
                WriteUsersToCsv(users);
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            List<User> users = ReadUsersFromCsv();
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role.ToString()); // Chuyển Enum thành chuỗi

                // Điều hướng theo vai trò
                if (user.Role == UserRole.Admin)
                    return RedirectToAction("Dashboard", "Admin");
                else if (user.Role == UserRole.Teacher)
                    return RedirectToAction("Dashboard", "Teacher");
                else
                    return RedirectToAction("Dashboard", "Student");
            }

            ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu!");
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private List<User> ReadUsersFromCsv()
        {
            List<User> users = new List<User>();
            if (!System.IO.File.Exists(filePath)) return users;

            var lines = System.IO.File.ReadAllLines(filePath);
            foreach (var line in lines.Skip(1)) // Bỏ qua dòng tiêu đề
            {
                var values = line.Split(',');

                users.Add(new User
                {
                    Id = int.Parse(values[0]),
                    FullName = values[1],
                    Address = values[2],
                    Username = values[3],
                    Password = values[4],

                    // Chuyển đổi Role từ chuỗi sang Enum
                    Role = Enum.TryParse(values[5], out UserRole role) ? role : UserRole.Student
                });
            }
            return users;
        }


        private void WriteUsersToCsv(List<User> users)
        {
            var lines = new List<string> { "Id,FullName,Address,Username,Password,Role" };
            lines.AddRange(users.Select(u => $"{u.Id},{u.FullName},{u.Address},{u.Username},{u.Password},{u.Role}"));

            System.IO.File.WriteAllLines(filePath, lines);
        }

    }
}
