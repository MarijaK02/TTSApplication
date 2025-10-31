using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Shared;
using TTS.Service.Interface;

namespace TTS.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> mailSettings)
        {
            _emailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string toFirstName, string toLastName, string subject, string userMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.EmailDisplayName, _emailSettings.SmtpUserName));
            emailMessage.To.Add(MailboxAddress.Parse(toEmail));
            emailMessage.Subject = subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = GetEmailHtml(userMessage, toFirstName + " " + toLastName)
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();


            try
            {
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    var socketOptions = SecureSocketOptions.Auto;

                    await smtp.ConnectAsync(_emailSettings.SmtpServer, 587, socketOptions);

                    if (!string.IsNullOrEmpty(_emailSettings.SmtpUserName))
                    {
                        await smtp.AuthenticateAsync(_emailSettings.SmtpUserName, _emailSettings.SmtpPassword);
                    }
                    await smtp.SendAsync(emailMessage);


                    await smtp.DisconnectAsync(true);
                }
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }

        private string GetEmailHtml(string userMessage, string name = "User")
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 0 10px rgba(0,0,0,0.1);
                    }}
                    .header {{
                        background-color: #0056b3;
                        color: white;
                        padding: 15px;
                        border-radius: 8px 8px 0 0;
                        text-align: center;
                        font-size: 24px;
                        font-weight: bold;
                    }}
                    .content {{
                        padding: 20px;
                        font-size: 16px;
                        color: #333333;
                        line-height: 1.5;
                    }}
                    .footer {{
                        text-align: center;
                        font-size: 12px;
                        color: #777777;
                        margin-top: 20px;
                    }}
                    a.button {{
                        display: inline-block;
                        padding: 10px 20px;
                        margin-top: 20px;
                        background-color: #0056b3;
                        color: white;
                        text-decoration: none;
                        border-radius: 5px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>TTS Application</div>
                    <div class='content'>
                        <p>Почитувани {name},</p>
                        <p>{userMessage}</p>
                        <p>Ви благодариме за користење на системот за трансфер на технологии</p>
                    </div>
                    <div class='footer'>
                        &copy; @{DateTime.Now.Year} All rights reserved.
                    </div>
                </div>
            </body>
            </html>
            ";
        }

    }
}
