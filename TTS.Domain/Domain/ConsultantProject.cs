using System;
using System.Collections.Generic;
using TTS.Domain.Enum;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class ConsultantProject : BaseEntity
    {
        public virtual required Consultant Consultant { get; set; }

        public virtual required Project Project { get; set; }

        public ICollection<Activity>? Activites { get; set; }

        public required DateTime DateApplied { get; set; }

        public DateTime? DateModified { get; set; }

        public ApplicationStatus ApplicationStatus { get; set; }
    }
}
