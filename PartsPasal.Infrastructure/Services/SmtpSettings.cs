namespace PartsPasal.Infrastructure.Services;

/// <summary>
/// SMTP configuration settings.
/// Values are stored in User Secrets (secret info like app password)
/// </summary>
public class SmtpSettings
{
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "PartsPasal";
}