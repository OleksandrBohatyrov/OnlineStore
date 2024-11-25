using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace OnlineStoreAPI.Services
{
    public class EmailService
    {
        private readonly string _fromEmail = "matveikulakovski@gmail.com";
        private readonly string _fromPassword = "xrzf pymd nlox vexs";

        public void SendPaymentConfirmationEmail(string customerEmail, string customerName, List<(string ProductName, int Quantity, decimal Price)> purchasedItems, decimal totalAmount)
        {
            try
            {
                // Address setup
                var fromAddress = new MailAddress(_fromEmail, "SolomikovPod Online Store");
                var toAddress = new MailAddress(customerEmail, customerName);

                // Building the purchase summary for the email
                string subject = "Successful Payment Confirmation";
                string body = $"Hello {customerName},\n\n" +
                              "Thank you for your purchase! Below is the summary of your order:\n\n";

                foreach (var item in purchasedItems)
                {
                    body += $"{item.ProductName} - Quantity: {item.Quantity}, Price per item: {item.Price.ToString("C")}\n";
                }

                body += $"\nTotal Amount: {totalAmount.ToString("C")}\n\n";
                body += "Your order is being processed, and we will notify you once it's shipped.\n";
                body += "Thank you for shopping with us!\n\nBest regards,\nSolomikovPod Online Store";

                // SMTP client setup
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, _fromPassword)
                };

                // Create the email message
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    // Send the email
                    smtp.Send(message);
                }

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email. Error: " + ex.Message);
            }
        }
    }
}
