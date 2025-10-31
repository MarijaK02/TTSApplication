using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO
{
    public class ConsultantProjectDto
    {
        public ConsultantProject Project { get; set; }
        public int TotalConsultantActivites { get; set; }
        public int TotalConsultantActiveActivites { get; set; }
        public int TotalConsultantFinishedActivites { get; set; }
        public int TotalConsultantInvalidActivities { get; set; }
        public int TotalConsultantNewActivities { get; set; }
        public int TotalHoursWorkingOnProject { get; set; }
    }
}
