using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Domain.Enum
{
    public enum TTSApplicationUserRole
    {
        [Display(Name = "Клиент")]
        Client,
        [Display(Name = "Консултант")]
        Consultant
    }
}
