using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Net.Mail;
using System.ComponentModel;

namespace WCS.Services.IPeople.Activities
{
    [Designer(typeof(EmailItemDesigner))]
    public sealed class SendEmail : CodeActivity
    {
        public InArgument<string> To { get; set; }

        public InArgument<string> Cc { get; set; }

        public InArgument<string> Bcc { get; set; }

        public InArgument<string> From { get; set; }

        public InArgument<string> Subject { get; set; }

        public InArgument<string> Body { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string to = context.GetValue(this.To);
            string cc = context.GetValue(this.Cc);
            string bcc = context.GetValue(this.Bcc);
            string from = context.GetValue(this.From);
            string subject = context.GetValue(this.Subject);
            string body = context.GetValue(this.Body);

            MailMessage message = new MailMessage();

            message.From = new MailAddress(from);

            message.To.Add(new MailAddress(to));

            if ((cc != null) && (cc != string.Empty))
            {
                message.CC.Add(new MailAddress(cc));
            }

            if ((bcc != null) && (bcc != string.Empty))
            {
                message.Bcc.Add(new MailAddress(bcc));
            }

            message.Subject = subject;

            message.Body = body;

            SmtpClient client = new SmtpClient("GCL1MS01.galwayclinic.net", 25);

            client.Send(message);
        }
    }
}
