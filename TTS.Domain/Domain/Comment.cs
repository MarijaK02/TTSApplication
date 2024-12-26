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
        public required TTSApplicationUser CreatedBy { get; set; }

        public required DateTime CreatedOn { get; set; }

        public required string CommentBody { get; set; }
        public required Activity Activity { get; set; }
        public List<Attachment>? Attachments { get; set; }
    }
}
