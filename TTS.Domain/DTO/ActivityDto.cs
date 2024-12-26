using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;

namespace TTS.Domain.DTO
{
    public class ActivityDto
    {
        public Guid ProjectId { get; set; }
        public Activity? Activity { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<Attachment>? Attachments { get; set; }
    }
}
