using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Domain.Enum
{
    public enum ApplicationStatus
    {
        [Display(Name = "Испратена")]
        Applied,

        [Display(Name = "Прифатена")]
        Accepted,

        [Display(Name = "Одбиена")]
        Rejected
    }
}
