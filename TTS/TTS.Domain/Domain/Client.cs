using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TTS.Domain.Enum;
using TTS.Domain.Identity;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Client : BaseEntity
    {
        public string UserId { get; set; }
        public TTSApplicationUser User { get; set; }

        public string? Address { get; set; }
        public Industry Industry { get; set; }

        public virtual ICollection<Project>? Projects { get; set; }
    }
}
