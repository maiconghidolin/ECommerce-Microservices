namespace NotificationService.Domain.Constants;

public static class MessagingSettings
{
    public const string OrderExchangeName = "order.events";
    public const string NotificationOrderCreatedQueueName = "notification.order.created";
    public const string OrderCreatedRoutingKey = "order.created";
}
