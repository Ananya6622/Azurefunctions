using System;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunctionApp
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("reset-password", Connection = "ConnectionString")]string body, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {body}");

            try
            {
                // Extract token and email from the message body
                // Assuming the message body is in the format "Token for reset-password: {token}, Email: {email}"
                string[] parts = body.Split(',');

                if (parts.Length >= 2)
                {
                    string token = parts[0].Split(':')[1].Trim();
                    string email = parts[1].Split(':')[1].Trim();

                    // Send email using SMTP
                    SendEmail(email, token, log);
                }
                else
                {
                    log.LogError("Invalid message format. Unable to extract token and email.");
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error processing Service Bus message: {ex.Message}");
            }
        }

        private void SendEmail(string email, string token, ILogger log)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                System.Net.NetworkCredential credential = new System.Net.NetworkCredential("ananyaetti6622@gmail.com", "oiyz onhn fcjd bhro");

                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = credential;
                MailMessage emailMessage = new MailMessage();
                emailMessage.From = new MailAddress("ananyaetti6622@gmail.com");
                emailMessage.To.Add(email);
                emailMessage.Subject = "FundooNotes reset Link";
                emailMessage.Body = $"This is your token for resetting the password: {token}";

                smtpClient.Send(emailMessage);

                log.LogInformation($"Email sent successfully to {email}.");
            }
            catch (Exception ex)
            {
                log.LogError($"Error sending email: {ex.Message}");
            }
        }
    }
}
