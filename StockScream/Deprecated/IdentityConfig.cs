namespace StockScream
{
    //public class EmailService : IIdentityMessageService
    //{
    //   public Task SendAsync(IdentityMessage message)
    //   {
    //      //await configSendGridasync(message);
    //       SendMail(message);
    //       return Task.FromResult(0);
    //   }

    //   void SendMail(IdentityMessage message)
    //   {
    //       #region formatter
    //       string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
    //       string html = "Please confirm your account by clicking this link: <a href=\"" + message.Body + "\">link</a><br/>";

    //       html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);
    //       #endregion

    //       var senderEmail = ConfigurationManager.AppSettings["adminEmail"];
    //       MailMessage msg = new MailMessage
    //       {
    //           From = new MailAddress(senderEmail),
    //           Subject = message.Subject
    //       };
    //       msg.To.Add(new MailAddress(message.Destination));
    //       msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
    //       msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

    //       var credentials = new NetworkCredential(
    //           ConfigurationManager.AppSettings["mailAccount"],
    //           ConfigurationManager.AppSettings["mailPassword"]);

    //       var smtpClient = new SmtpClient
    //       {
    //           Host = ConfigurationManager.AppSettings["smtpServer"],
    //           Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]),
    //           EnableSsl = bool.Parse(ConfigurationManager.AppSettings["enableSsl"]),
    //           DeliveryMethod = SmtpDeliveryMethod.Network,
    //           Credentials = credentials
    //       };
    //       smtpClient.Send(msg);
    //   }


    //   // Use NuGet to install SendGrid (Basic C# client lib) 
    //   private async Task configSendGridasync(IdentityMessage message)
    //   {
    //      var myMessage = new SendGridMessage();
    //      myMessage.AddTo(message.Destination);
    //      myMessage.From = new System.Net.Mail.MailAddress(
    //                          "Joe@contoso.com", "Joe S.");
    //      myMessage.Subject = message.Subject;
    //      myMessage.Text = message.Body;
    //      myMessage.Html = message.Body;

    //      var credentials = new NetworkCredential(
    //                 ConfigurationManager.AppSettings["mailAccount"],
    //                 ConfigurationManager.AppSettings["mailPassword"]
    //                 );

    //      // Create a Web transport for sending email.
    //      var transportWeb = new Web(credentials);

    //      try
    //      {
    //         // Send the email.
    //         if (transportWeb != null)
    //         {
    //            await transportWeb.DeliverAsync(myMessage);
    //         }
    //         else
    //         {
    //            Trace.TraceError("Failed to create Web transport.");
    //            await Task.FromResult(0);
    //         }
    //      }
    //      catch (Exception ex)
    //      {
    //         Trace.TraceError(ex.Message + " SendGrid probably not configured correctly.");
    //      }
    //   }
    //}

    //public class SmsService : IIdentityMessageService
    //{
    //   public Task SendAsync(IdentityMessage message)
    //   {
    //      var Twilio = new TwilioRestClient(
    //         ConfigurationManager.AppSettings["TwilioSid"],
    //         ConfigurationManager.AppSettings["TwilioToken"]
    //     );
    //      var result = Twilio.SendMessage(
    //          ConfigurationManager.AppSettings["TwilioFromPhone"],
    //         message.Destination, message.Body);

    //      // Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
    //      Trace.TraceInformation(result.Status);

    //      // Twilio doesn't currently have an async API, so return success.
    //      return Task.FromResult(0);
    //   }
    //}

    //public class ApplicationUserManager : UserManager<ApplicationUser>
    //{
    //    public ApplicationUserManager(IUserStore<ApplicationUser> store)
    //        : base(store)
    //    {
    //    }

    //    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
    //    {
    //        var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
    //        // Configure validation logic for usernames
    //        manager.UserValidator = new UserValidator<ApplicationUser>(manager)
    //        {
    //            AllowOnlyAlphanumericUserNames = false,
    //            RequireUniqueEmail = true
    //        };

    //        // Configure validation logic for passwords
    //        //manager.PasswordValidator = new PasswordValidator
    //        //{
    //        //    RequiredLength = 6,
    //        //    RequireNonLetterOrDigit = true,
    //        //    RequireDigit = true,
    //        //    RequireLowercase = true,
    //        //    RequireUppercase = true,
    //        //};

    //        // Configure user lockout defaults
    //        manager.UserLockoutEnabledByDefault = true;
    //        manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //        manager.MaxFailedAccessAttemptsBeforeLockout = 5;

    //        // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
    //        // You can write your own provider and plug it in here.
    //        manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
    //        {
    //            MessageFormat = "Your security code is {0}"
    //        });
    //        manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
    //        {
    //            Subject = "Security Code",
    //            BodyFormat = "Your security code is {0}"
    //        });
    //        manager.EmailService = new EmailService();
    //        manager.SmsService = new SmsService();
    //        var dataProtectionProvider = options.DataProtectionProvider;
    //        if (dataProtectionProvider != null)
    //        {
    //            manager.UserTokenProvider =
    //                new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
    //        }
    //        return manager;
    //    }
    //}


    //// Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    //public class ApplicationRoleManager : RoleManager<IdentityRole>
    //{
    //    public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
    //        : base(roleStore)
    //    {
    //    }

    //    public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
    //    {
    //        return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
    //    }
    //}

    //// Configure the application sign-in manager which is used in this application.
    //public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    //{
    //   public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
    //      : base(userManager, authenticationManager)
    //   {
    //   }

    //   public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
    //   {
    //      return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
    //   }

    //   public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
    //   {
    //      return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
    //   }
    //}
}
