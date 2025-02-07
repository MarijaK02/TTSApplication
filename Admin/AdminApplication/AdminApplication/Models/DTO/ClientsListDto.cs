using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO
{
    public class ClientsListDto
    {
        public List<Client> Clients { get; set; }
        public Industry? SelectedIndustry { get; set; }
        public string? SearchTerm { get; set; }
    }
}
