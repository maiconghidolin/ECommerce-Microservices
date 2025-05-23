namespace NotificationService.Application.Models;

public class SMS
{
    public string UserId { get; set; }
    public string Number { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
