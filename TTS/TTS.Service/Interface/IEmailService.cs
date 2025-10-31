using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Service.Interface
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string toFirstName, string toLastName, string subject, string userMessage);
    }
}
