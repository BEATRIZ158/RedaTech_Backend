using Redatech.Enums;

namespace Redatech.Models
{
    public class Correction
    {
        public int Id { get; set; }
        public int EssayId { get; set; }
        public int TeacherId { get; set; }
        public StatusCorrection Status { get; set; }
        public string Feedback { get; set; }
        public int Score { get; set; }
        public DateTime CorrectedAt { get; set; }
    }
}
