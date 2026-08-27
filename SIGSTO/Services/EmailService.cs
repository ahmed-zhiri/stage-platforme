using System.Net;
using System.Net.Mail;

namespace Tremplin.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void EnvoyerOTP(string destinataire, string otpCode)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"] ?? "587");
            var smtpUser = _config["Smtp:User"];
            var smtpPass = _config["Smtp:Password"];
            var fromEmail = _config["Smtp:From"];

            if (string.IsNullOrEmpty(smtpPass))
                throw new Exception("Le mot de passe SMTP n'est pas configure. Verifiez appsettings.Development.json");

            var message = new MailMessage();
            message.From = new MailAddress(fromEmail!, "Tremplin - ONEE");
            message.To.Add(destinataire);
            message.Subject = "Tremplin - Code de verification OTP";
            message.IsBodyHtml = true;
            message.Body = $@"
                <div style='font-family: Segoe UI, sans-serif; max-width: 500px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #0d6efd;'>Tremplin - Verification de votre email</h2>
                    <p>Bonjour,</p>
                    <p>Votre code de verification OTP est :</p>
                    <div style='background: #f0f4ff; border: 2px solid #0d6efd; border-radius: 8px; padding: 20px; text-align: center; margin: 20px 0;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #0d6efd;'>{otpCode}</span>
                    </div>
                    <p>Ce code expire dans <strong>10 minutes</strong>.</p>
                    <p style='color: #888; font-size: 13px;'>Si vous n'avez pas demande ce code, ignorez cet email.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;' />
                    <p style='color: #aaa; font-size: 12px;'>Tremplin - Plateforme de gestion des stages - ONEE</p>
                </div>";

            using var client = new SmtpClient(smtpHost, smtpPort);
            client.Credentials = new NetworkCredential(smtpUser, smtpPass);
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Timeout = 15000;
            client.Send(message);
        }
    }
}
