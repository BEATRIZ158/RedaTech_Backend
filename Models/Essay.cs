using System.Globalization;

namespace Redatech.Models
{
    public class Essay
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
