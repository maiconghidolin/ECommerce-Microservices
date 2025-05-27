using EasyNetQ;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Services;
using NotificationService.Domain.Events;
using NotificationService.Domain.Interfaces.MqConsumer;

namespace NotificationService.Application.MqConsumer;

public class MessageHandler : IMessageHandler
{
    private readonly ILogger<MessageHandler> _logger;
    private readonly AbstractNotification<Models.Email> _emailNotification;
    private readonly AbstractNotification<Models.SMS> _smsNotification;

    public MessageHandler(AbstractNotification<Models.Email> emailNotification, AbstractNotification<Models.SMS> smsNotification, ILogger<MessageHandler> logger)
    {
        _emailNotification = emailNotification;
        _smsNotification = smsNotification;
        _logger = logger;
    }

    public async Task SendOrderCreatedNotification(OrderCreated message, MessageProperties messageProperties, MessageReceivedInfo receivedInfo)
    {
        _logger.LogInformation($"Received OrderCreated message: {message.Id}");

        List<Task> tasks = new();

        if (!string.IsNullOrEmpty(message.UserEmail))
        {
            Models.Email email = new()
            {
                UserId = message.UserId,
                EmailAdress = message.UserEmail,
                Subject = "New order created",
                Body = $"You have a new order created with id {message.Id}."
            };

            var taskEmail = _emailNotification.Create(email);
            tasks.Add(taskEmail);
        }

        if (!string.IsNullOrEmpty(message.UserNumber))
        {
            Models.SMS sms = new()
            {
                UserId = message.UserId,
                Number = message.UserNumber,
                Subject = "New order created",
                Body = $"You have a new order created with id {message.Id}."
            };

            var taskSMS = _smsNotification.Create(sms);
            tasks.Add(taskSMS);
        }

        await Task.WhenAll(tasks);
    }
}
