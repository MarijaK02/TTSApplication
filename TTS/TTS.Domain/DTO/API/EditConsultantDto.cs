using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Enum;

namespace TTS.Domain.DTO.API
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
