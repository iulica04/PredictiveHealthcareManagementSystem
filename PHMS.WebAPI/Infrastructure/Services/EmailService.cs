﻿using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Services;

public class EmailService : IEmailService
{
    private readonly string smtpServer = "smtp.gmail.com";
    private readonly int smtpPort = 587;
    private readonly bool enableSsl = true;
    private readonly string senderEmail = "predictsmarthealth@gmail.com"; 
    private readonly string senderPassword = "rddl zyvx zjpv auxd"; 

    public async Task SendEmailAsync(string recipientEmail, string subject, string message)
    {
        var client = new SmtpClient(smtpServer, smtpPort)
        {
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(senderEmail, senderPassword)
        };

        var mailMessage = new MailMessage(senderEmail, recipientEmail, subject, message)
        {
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8,
            IsBodyHtml = true
        };

        await client.SendMailAsync(mailMessage);
    }
}
