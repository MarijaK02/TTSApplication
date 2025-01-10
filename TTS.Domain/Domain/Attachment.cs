using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Domain.Domain
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; } 
        public required string FilePath { get; set; }
        public required virtual Comment Comment { get; set; }
    }
}
