namespace DemoAPI.Models
{
    public class Semester
    {
        public string NumberOfSemester { get; set; }
        public string SemesterName { get; set; }
        public int NumberOfNewStudents { get; set; }
        public int NumberOfAllStudents { get; set; }
        public int NumberOfTeachers { get; set; }
        public int NumberOfStaffs { get; set; }
        public int Id { get; internal set; }
    }
}
