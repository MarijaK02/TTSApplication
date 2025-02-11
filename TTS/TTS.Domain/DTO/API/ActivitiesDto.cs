using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;

namespace TTS.Domain.DTO.API
{
    public class ActivitiesDto
    {
        public List<Activity> Activities { get; set; }
        public List<ConsultantProject> Consultants { get; set; }
    }
}
