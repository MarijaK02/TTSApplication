using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Enum;

namespace TTS.Domain.DTO
{
    public class CreateCompanyDto
    {
        public string? Address { get; set; }
        public required string Name { get; set; }
        [EnumDataType(typeof(Industry))]
        public required Industry Industry { get; set; }
        [Phone]
        public string? ContactPhone { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
    }
}
