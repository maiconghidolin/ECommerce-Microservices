namespace OrderService.Domain.Constants;

public static class MessagingSettings
{
    public const string OrderExchangeName = "order.events";
    public const string OrderCreatedRoutingKey = "order.created";
}
