using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Identity;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Comment : BaseEntity
    {
        public string CreatedById { get; set; }
        public TTSApplicationUser? CreatedBy { get; set; }
        public Guid ActivityId { get; set; }
        public Activity? Activity { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CommentBody { get; set; }

        public virtual List<Attachment>? Attachments { get; set; }
    }
}
