using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.Enum;

namespace TTS.Domain.DTO
{
    public class IndexProjectsDto
    {
        public List<Project> Projects { get; set; }
        public string? SearchTerm { get; set; }
        public Expertise? SelectedExpertise { get; set; }
    }
}
