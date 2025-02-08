using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO.API
{
    public class EditClientDto
    {
        public Guid ClientId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public Industry Industry { get; set; }

    }
}
