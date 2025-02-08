using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string UserId { get; set; }
        public TTSApplicationUser User { get; set; }

        public Expertise Expertise { get; set; }

        public virtual ICollection<ConsultantProject>? Projects { get; set; }  
    }
}
