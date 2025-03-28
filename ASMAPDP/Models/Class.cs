namespace ASMAPDP.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string TeacherUsername { get; set; } // Liên kết với giáo viên
        public string Schedule { get; set; } // Thời khóa biểu (ví dụ: "Thứ 2, 8:00-10:00")
    }
}
