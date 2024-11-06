using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Enum;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Company : BaseEntity
    {
        public string? Address { get; set; }
        public required string Name { get; set; }
        public required Industry Industry { get; set; }
        public string? ContactPhone { get; set; }
        public string? Email { get; set; }
    }
}
