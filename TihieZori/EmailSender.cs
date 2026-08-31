// Создай интерфейс и реализацию
public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogInformation($"Sending email to {email}: {subject}");
        // Здесь добавь реальную отправку через SMTP или SendGrid
        return Task.CompletedTask;
    }
}