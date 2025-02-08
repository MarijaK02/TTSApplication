namespace AdminApplication.Models.DTO.API
{
    public class AddConsultantToProjectDto
    {
        public Guid ProjectId { get; set; }
        public Guid ConsultantId { get; set; }
    }
}
