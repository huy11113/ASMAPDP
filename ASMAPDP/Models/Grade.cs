namespace ASMAPDP.Models
{
    public class Grade
    {
        public int ClassId { get; set; } // ID của lớp học
        public string StudentUsername { get; set; } // Username của sinh viên
        public double Score { get; set; } // Điểm số
        public string TeacherUsername { get; set; } // Username của giáo viên nhập điểm
    }
}
