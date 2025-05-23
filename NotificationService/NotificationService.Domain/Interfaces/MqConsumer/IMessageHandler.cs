using EasyNetQ;
using NotificationService.Domain.Events;

namespace NotificationService.Domain.Interfaces.MqConsumer;

public interface IMessageHandler
{
    Task SendOrderCreatedNotification(OrderCreated message, MessageProperties messageProperties, MessageReceivedInfo receivedInfo);
}
