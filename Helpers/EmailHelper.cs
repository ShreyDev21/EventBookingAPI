using System.Net;
using System.Net.Mail;

public static class EmailHelper
{
    public static async Task<(bool Success, string Message)> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            var fromEmail = "shreyporwal01@gmail.com";
            var fromPassword = "hwuisjcwthifdjor";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, fromPassword)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
            return (true, "Email sent successfully.");
        }
        catch (SmtpFailedRecipientException ex)
        {
            return (false, $"Failed to deliver to recipient: {ex.FailedRecipient}. Reason: {ex.Message}");
        }
        catch (SmtpException ex)
        {
            return (false, $"SMTP error: {ex.StatusCode} - {ex.Message}");
        }
        catch (FormatException)
        {
            return (false, "Invalid email format.");
        }
        catch (Exception ex)
        {
            return (false, $"Unexpected error: {ex.Message}");
        }
    }
}
