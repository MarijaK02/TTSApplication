using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Domain.Enum
{
    public enum ProjectStatus
    {
        //klient podnesuva baranje za proekt
        New,
        //Koga ke bide kreirana prvata aktivnost (ili koga konsultantot i adminot ke smenat znaci moze i bez aktivnosti)
        [Display(Name = "In Progress")]
        InProgress,
        //Koga ke se zavrsi proektot (adminot i konsultantot go menuvaat statusot)
        Completed,
        //Dokolku odluci adminot deka e nevaliden
        Invalid
    }
}
