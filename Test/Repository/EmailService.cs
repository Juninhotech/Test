using Azure;
using Hangfire;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Test.Repository
{
    public class EmailService
    {
        IHostEnvironment _environment;
        IConfiguration _configuration;

        public EmailService(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void SendEmail(string EmailTo, string name)
        {
            try
            {
                
                string mailFrom = _configuration["EmailSettings:MailFrom"];
                string mailFromName = _configuration["EmailSettings:MailFromName"];
                int smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort");
                string smtpServer = _configuration["EmailSettings:SmtpServer"];
                bool enableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl");
                string smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                string smtpPassword = _configuration["EmailSettings:SmtpPassword"];


                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(mailFrom, mailFromName);
                    message.To.Add(new MailAddress(EmailTo));
                    message.Subject = "SECUREID LTD";
                    message.IsBodyHtml = true;
                    message.Body = $"Hi {name}, Welcome to SECUREID LTD";
                    message.IsBodyHtml = true;

                    using (var client = new SmtpClient(smtpServer, smtpPort))
                    {
                        client.EnableSsl = enableSsl;
                        client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        client.Send(message);
                    }

                }
            }
            catch (Exception ex)
            {

                BackgroundJob.Schedule(() => SendEmail(EmailTo, name), TimeSpan.FromMinutes(5));

            }
           

        }
    }
}
