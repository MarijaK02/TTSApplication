using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Domain.Enum
{
    public enum ProjectStatus
    {
        //klient podnesuva baranje za proekt
        New,
        //koga ke bide prv pat prifaten od nekoj konsultant
        Accepted,
        //Koga ke bide kreirana prvata aktivnost
        InProgress,
        //Koga ke se zavrsi proektot
        Completed,
        //Dokolku klientot ili adminot go izbrisat proektot
        Deleted,
        //Dokolku odluci adminot deka e nevaliden
        Invalid
    }
}
