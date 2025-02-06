using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Attachment : BaseEntity
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}
