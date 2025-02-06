using System;
using System.Collections.Generic;
using TTS.Domain.Enum;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class ConsultantProject : BaseEntity
    {
        public Guid ConsultantId { get; set; }
        public Consultant? Consultant { get; set; }
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }  
        public virtual ICollection<Activity>? Activites { get; set; }  

        public DateTime DateApplied { get; set; }
        public DateTime? DateModified { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
    }

}
