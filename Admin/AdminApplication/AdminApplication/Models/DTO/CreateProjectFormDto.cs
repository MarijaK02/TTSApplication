using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO
{
    public class CreateProjectFormDto
    {
        public Project Project { get; set; }

        public List<Client> Clients { get; set; }
    }
}
