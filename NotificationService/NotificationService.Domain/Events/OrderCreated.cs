namespace NotificationService.Domain.Events;

public class OrderCreated
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public string UserNumber { get; set; }

}

