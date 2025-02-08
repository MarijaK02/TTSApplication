using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO
{
    public class ConsultantsListDto
    {
        public List<Consultant> Consultants { get; set; }
        public string? SearchTerm { get; set; }
        public Expertise? SelectedExpertise { get; set; }
    }
}
