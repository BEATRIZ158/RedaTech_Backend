namespace Redatech.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TeacherId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public List<ClassStudent> ClassStudents { get; set; } = new();
    }
}
