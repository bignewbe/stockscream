using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace StockScream.Identity
{
    public class EmailService : IIdentityMessageService
    {
        static bool _isInitialized;
        static SmtpClient _emailClient;
        static AutoResetEvent _event = new AutoResetEvent(true);
        static string _fromEmail;

        public static void Initialize(string host, int port, string emailAccount, string password, bool enableSsl, string fromEmail)
        {
            if (_isInitialized) return;

            try
            {
                _event.WaitOne();
                _fromEmail = fromEmail;
                _emailClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential
                    {
                        UserName = emailAccount,
                        Password = password
                    },
                };
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                _event.Set();
            }
        }

        public static async Task SendEmail(string destination, string subject, string body)
        {
            try {
                _event.WaitOne();
                var msg = new MailMessage
                {
                    From = new MailAddress(_fromEmail),
                    Subject = subject,
                    Body = body,
                };
                msg.To.Add(new MailAddress(destination));

                await Task.Run(() => _emailClient.Send(msg));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                _event.Set();
            }      

            //var smtpClient = new SmtpClient
            //{
            //    Host = ConfigurationManager.AppSettings["smtpServer"],
            //    Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]),
            //    EnableSsl = bool.Parse(ConfigurationManager.AppSettings["enableSsl"]),
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    Credentials = new NetworkCredential
            //    {
            //        UserName = ConfigurationManager.AppSettings["mailAccount"],
            //        Password = ConfigurationManager.AppSettings["mailPassword"]
            //    },
            //};
            //smtpClient.Send(msg);
        }

        public Task SendAsync(IdentityMessage message)
        {
            //await configSendGridasync(message);
            SendMail(message);
            return Task.FromResult(0);
        }

        void SendMail(IdentityMessage message)
        {
            #region formatter
            string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
            string html = "Please confirm your account by clicking this link: <a href=\"" + message.Body + "\">link</a><br/>";

            html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);
            #endregion

            var senderEmail = ConfigurationManager.AppSettings["adminEmail"];
            MailMessage msg = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = message.Subject
            };
            msg.To.Add(new MailAddress(message.Destination));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]);

            var smtpClient = new SmtpClient
            {
                Host = ConfigurationManager.AppSettings["smtpServer"],
                Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]),
                EnableSsl = bool.Parse(ConfigurationManager.AppSettings["enableSsl"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = credentials
            };
            smtpClient.Send(msg);
        }
    }
}
