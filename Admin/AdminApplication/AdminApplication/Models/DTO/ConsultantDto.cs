namespace AdminApplication.Models.DTO
{
    public class ConsultantDto
    {
        public Consultant Consultant { get; set; }
        public List<ConsultantProjectDto> Projects { get; set; }
    }
}
