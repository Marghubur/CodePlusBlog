namespace CodePlusBlog.IService
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string ReceiverEmail, string Subject, string body);
    }
}
