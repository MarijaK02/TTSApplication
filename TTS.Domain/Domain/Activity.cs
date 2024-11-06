using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Enum;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Activity : BaseEntity
    {
        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }

        [Required]
        public required ActivityStatus Status { get; set; }

        [Required]
        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public required Consultant CreatedBy { get; set; }

        [Required]
        public required Project Project { get; set; }
    }
}
