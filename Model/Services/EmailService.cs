using Microsoft.AspNet.Identity;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace StockScream.Identity
{
    public class EmailService : IIdentityMessageService
    {
        SmtpClient _smtpClient;
        string _senderEmail;

        public EmailService(string host, int port, string senderEmail, string account, string password, bool enableSsl)
        {
            _senderEmail = senderEmail;
            _smtpClient = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential
                {
                    UserName = account,
                    Password = password
                },
            };            
        }

        public Task SendAsync(IdentityMessage message)
        {
            //string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
            //string html = "Please confirm your account by clicking this link: <a href=\"" + message.Body + "\">link</a><br/>";
            //html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);
            var msg = new MailMessage
            {
                From = new MailAddress(_senderEmail),
                Subject = message.Subject
            };
            msg.To.Add(new MailAddress(message.Destination));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.Body, null, MediaTypeNames.Text.Plain));
            
            //msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            _smtpClient.Send(msg);            
            return Task.FromResult(0);
        }

        public async Task SendEmail(string destination, string subject, string body, string senderEmail="nobody")
        {
            var msg = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = body,
            };
            msg.To.Add(new MailAddress(destination));
            await Task.Run(() => _smtpClient.Send(msg));
        }

        
        //public static void SendMail(IdentityMessage message, 
        //    string senderEmail, 
        //    string account, 
        //    string password,
        //    string smtpServer,
        //    int port, 
        //    bool enableSsl)
        //{
        //    #region formatter
        //    string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
        //    string html = "Please confirm your account by clicking this link: <a href=\"" + message.Body + "\">link</a><br/>";

        //    html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);
        //    #endregion

        //    MailMessage msg = new MailMessage
        //    {
        //        From = new MailAddress(senderEmail),
        //        Subject = message.Subject
        //    };
        //    msg.To.Add(new MailAddress(message.Destination));
        //    msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
        //    msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

        //    var credentials = new NetworkCredential(account, password);

        //    var smtpClient = new SmtpClient
        //    {
        //        Host = smtpServer,
        //        Port = port,
        //        EnableSsl = enableSsl,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        Credentials = credentials
        //    };
        //    smtpClient.Send(msg);
        //}
    }
}
