using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using System.Net.Mime;

namespace StockScream.Identity
{
    public class SmsService : IIdentityMessageService
   {
      public Task SendAsync(IdentityMessage message)
      {
        // var Twilio = new TwilioRestClient(
        //    ConfigurationManager.AppSettings["TwilioSid"],
        //    ConfigurationManager.AppSettings["TwilioToken"]
        //);
        // var result = Twilio.SendMessage(
        //     ConfigurationManager.AppSettings["TwilioFromPhone"],
        //    message.Destination, message.Body);

        // // Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
        // Trace.TraceInformation(result.Status);

         // Twilio doesn't currently have an async API, so return success.
         return Task.FromResult(0);
      }
   }
}
