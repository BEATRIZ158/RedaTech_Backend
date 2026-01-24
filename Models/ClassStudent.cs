namespace Redatech.Models
{
    public class ClassStudent
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; }

        public User Student { get; set; }
    }
}
