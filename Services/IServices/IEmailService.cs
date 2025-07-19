namespace itRoot.Services.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailConfirmationAsync(string email, string fullName, string confirmationUrl);
    }
}
