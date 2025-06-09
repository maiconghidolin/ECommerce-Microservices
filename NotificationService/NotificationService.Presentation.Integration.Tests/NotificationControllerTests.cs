using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure;
using NotificationService.Presentation.Integration.Tests.Factory;
using System.Net;
using System.Net.Http.Json;

namespace NotificationService.Presentation.Integration.Tests;

public class NotificationControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly IMongoCollection<Notification> _notificationCollection;

    public NotificationControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        _notificationCollection = context.Notifications;
    }

    [Fact]
    public async Task GetAll_Should_Return_EmptyList_When_NoNotifications()
    {
        // Arrange
        await _notificationCollection.DeleteManyAsync(FilterDefinition<Notification>.Empty);

        // Act
        var response = await _client.GetAsync("/notifications");

        // Assert
        response.EnsureSuccessStatusCode();

        var notifications = await response.Content.ReadFromJsonAsync<List<Notification>>();

        Assert.Empty(notifications);
    }

    [Fact]
    public async Task GetAll_Should_Return_Notification_When_Exists()
    {
        // Arrange
        var notification = new Notification { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Type = "Email", EmailAdress = "test", Subject = "test", Body = "test" };
        await _notificationCollection.InsertManyAsync(new[]
        {
            notification
        });

        // Act
        var response = await _client.GetAsync("/notifications");

        // Assert
        response.EnsureSuccessStatusCode();

        var notifications = await response.Content.ReadFromJsonAsync<List<Notification>>();

        Assert.NotEmpty(notifications);
        Assert.Contains(notifications, n => n.Id == notification.Id);
    }

    [Fact]
    public async Task CreateEmailNotification_Should_Return_Created()
    {
        // Arrange
        Application.Models.Email email = new()
        {
            EmailAdress = "test",
            Body = "Test Body",
            Subject = "Test Subject",
            UserId = Guid.NewGuid().ToString()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/notifications/email", email);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateSMSNotification_Should_Return_Created()
    {
        // Arrange
        Application.Models.SMS sms = new()
        {
            Number = "1234567890",
            Body = "Test SMS Body",
            Subject = "Test SMS Subject",
            UserId = Guid.NewGuid().ToString()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/notifications/sms", sms);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
