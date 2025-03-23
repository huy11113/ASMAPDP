namespace ASMAPDP.Models
{
    public class Course : IPrintable
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Location { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<Student> Students { get; set; }
        public string printInfo()
        {
            throw new NotImplementedException();
        }
    }
}
