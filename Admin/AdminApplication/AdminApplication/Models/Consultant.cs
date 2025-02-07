using AdminApplication.Models.Enums;

namespace AdminApplication.Models
{
    public class Consultant
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public TTSApplicationUser User { get; set; }

        public Expertise Expertise { get; set; }

        public ICollection<ConsultantProject>? Projects { get; set; }
    }
}
