using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Enum;
using TTS.Domain.Identity;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Consultant : BaseEntity
    {
        public required TTSApplicationUser User { get; set; }
        public required Expertise Expertise { get; set; }
    }
}
