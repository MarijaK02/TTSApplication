using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO.API
{
    public class EditConsultantDto
    {
        public Guid ConsultantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Expertise Expertise { get; set; }
    }
}
