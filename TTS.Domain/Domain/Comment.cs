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
        [Required]
        public required TTSApplicationUser CreatedBy { get; set; }

        [Required]
        public required DateTime CreatedOn { get; set; }

        [Required]
        public required string CommentBody { get; set; }
    }
}
