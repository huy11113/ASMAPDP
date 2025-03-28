namespace ASMAPDP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Login}/{id?}");

                endpoints.MapControllerRoute(
                    name: "admin_dashboard",
                    pattern: "admin/dashboard",
                    defaults: new { controller = "Admin", action = "Dashboard" });

                endpoints.MapControllerRoute(
                    name: "student_dashboard",
                    pattern: "student/dashboard",
                    defaults: new { controller = "Student", action = "Dashboard" });

                endpoints.MapControllerRoute(
                    name: "teacher_dashboard",
                    pattern: "teacher/dashboard",
                    defaults: new { controller = "Teacher", action = "Dashboard" });

                endpoints.MapControllerRoute(
                    name: "teacher_classes",
                    pattern: "teacher/classes",
                    defaults: new { controller = "Classes", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "teacher_classes_add",
                    pattern: "teacher/classes/add",
                    defaults: new { controller = "Classes", action = "Add" });

                endpoints.MapControllerRoute(
                    name: "teacher_grades",
                    pattern: "teacher/grades/class/{classId}",
                    defaults: new { controller = "Grades", action = "EnterGrades" });

                endpoints.MapControllerRoute(
                    name: "teacher_grades_save",
                    pattern: "teacher/grades/save",
                    defaults: new { controller = "Grades", action = "SaveGrades" });

                endpoints.MapControllerRoute(
                    name: "teacher_grades_index",
                    pattern: "teacher/grades",
                    defaults: new { controller = "Grades", action = "Index" });

                // C?p nh?t route cho TeacherStudentListController
                endpoints.MapControllerRoute(
                    name: "teacher_students",
                    pattern: "teacher/students",
                    defaults: new { controller = "TeacherStudentList", action = "Index" });
            });

            app.Run();
        }
    }
}
