using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Form.Core.Pipelines.ProcessMessage;
using System.Net.Mail;

namespace Elma.SitecoreUtil.Pipelines.ProcessMessage
{
    public class ProcessMessage
    {
        public void BuildToFromDivision(ProcessMessageArgs args)
        {
            var divisionDropList = args.Fields.Where(f => Sitecore.Context.Database.GetItem(f.FieldID).Fields["Field Link"].Value.Equals(Constants.WFM_DivisionDropList_Id, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (divisionDropList != null)
            {
                // clear existing `To` value
                args.To.Remove(0, args.To.Length);

                // get all comma and semi-colon separated emails from the division
                string[] emails = divisionDropList.Value.Replace(" ", "").Split(new char[] {',',';'});

                int totalEmailCount = emails.Length;
                int currentEmailNumber = 1;

                foreach (string e in emails)
                {
                    args.To.Append(e);

                    // delimit so we know how to break up emails in a later step
                    if(currentEmailNumber != totalEmailCount)
                        args.To.Append("|");

                    currentEmailNumber++;
                }
            }
        }

        public void SendEmail(ProcessMessageArgs args)
        {
            SmtpClient smtpClient = new SmtpClient(args.Host);
            smtpClient.EnableSsl = args.EnableSsl;
            if (args.Port != 0)
            {
                smtpClient.Port = args.Port;
            }
            smtpClient.Credentials = args.Credentials;
            smtpClient.Send(GetMail(args));
        }

        private MailMessage GetMail(ProcessMessageArgs args)
        {
            MailMessage mail = new MailMessage();

            string[] emails = args.To.ToString().Split('|');

            foreach (string address in emails)
            {
                mail.To.Add(address);
            }

            mail.From = new MailAddress(args.From);
            mail.Subject = args.Subject.ToString();
            mail.Body = args.Mail.ToString();
            mail.IsBodyHtml = args.IsBodyHtml;
            
            
            if (args.CC.Length > 0)
            {
                mail.CC.Add(args.CC.ToString());
            }
            if (args.BCC.Length > 0)
            {
                mail.Bcc.Add(args.BCC.ToString());
            }
            args.Attachments.ForEach(delegate(Attachment attachment)
            {
                mail.Attachments.Add(attachment);
            });
            return mail;
        }
    }
}
