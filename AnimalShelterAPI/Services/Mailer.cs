using AnimalShelterAPI.Models;
using AnimalShelterAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services
{
    public class Mailer : IMailer
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IConfiguration _configuration;

        public Mailer(IConfiguration configuration, IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
            _configuration = configuration;
        }

        private async Task Send(MimeMessage message)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(_smtpSettings.ServerName, _smtpSettings.Port, true);

                    await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            message.To.Add(new MailboxAddress("AnimalShelter", email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };

            await Send(message);
        }

        public async Task SendMassEmailAsync(List<string> recipients, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            foreach (string email in recipients)
            {
                message.To.Add(new MailboxAddress("AnimalShelter", email));
            }
            
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };

            await Send(message);
        }
    }
}
