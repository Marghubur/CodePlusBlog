using CodePlusBlog.IService;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ServiceLayer.Service
{
    public class EmailService: IEmailService
    {
        private SmtpClient _client;
        public StringBuilder _body;

        public EmailService()
        {
            _client = new SmtpClient();
        }
        public async Task<bool> SendEmail(string ReceiverEmail, string Subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(ReceiverEmail);
                mail.Subject = Subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                mail.From = new MailAddress("marghub12@rediffmail.com", "Testing", Encoding.UTF8);
                _client.Port = 587;
                _client.Host = "smtp.rediffmail.com";
                _client.Credentials = new NetworkCredential("marghub12@rediffmail.com", "q1mbtjgxhv4okg0kog");
                _client.EnableSsl = true;
                _client.UseDefaultCredentials = false;
                await _client.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
    }
}
