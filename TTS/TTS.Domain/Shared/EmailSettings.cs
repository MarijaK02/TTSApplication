using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Domain.Shared
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpServerPort { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public bool EnableSsl { get; set; }
        public string EmailDisplayName { get; set; }
        public string SendersName { get; set; }
    }
}
