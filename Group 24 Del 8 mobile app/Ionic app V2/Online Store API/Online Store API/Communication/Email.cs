using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Online_Store_API.Communication
{
    public static class Email
    {
        //const string ApiKey = "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";
        //private const string ApiKey = "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";
        private const string ApiKey = "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";

        private static async Task PlaceOrderTask(string _toEmail, string _toName, string message)
        {
            var client = new SendGridClient(ApiKey);

            var from = new EmailAddress("sharont.marecha@gmail.com", "Dippenaar & Reinecke");
            var subject = "[ Dippenaar & Reinecke] New Order";
            var to = new EmailAddress(_toEmail.ToString(), _toName.ToString());
            var plainTextContent = "Order confirmation";
            var htmlContent = "Hello " + _toName + ", <br/><br/>" + message + " <br/><br/>Regards,<br/>Dippenaar & Reinecke Team";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
        public static void SendPlaceOrderEmail(string toEmail, string toName, string message)
        {
            {
                PlaceOrderTask(toEmail, toName, message).Wait();
            }
        }
    }
}