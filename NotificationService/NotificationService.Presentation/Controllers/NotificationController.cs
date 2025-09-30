using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Models;
using NotificationService.Application.Services;

namespace NotificationService.Presentation.Controllers;

[ApiController]
[Authorize(Policy = "AdminOrNotificationManager")]
[Route("notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly AbstractNotification<Email> _emailNotification;
    private readonly AbstractNotification<SMS> _smsNotification;

    public NotificationController(
        INotificationService notificationService,
        AbstractNotification<Email> emailNotification,
        AbstractNotification<SMS> smsNotification)
    {
        _notificationService = notificationService;
        _emailNotification = emailNotification;
        _smsNotification = smsNotification;
    }

    [HttpGet]
    public async Task<List<Notification>> Get(int offset, int fetch)
    {
        return await _notificationService.GetAll(offset, fetch);
    }

    [HttpPost("email")]
    public async Task<ActionResult> CreateEmailNotification([FromBody] Email emailNotification)
    {
        await _emailNotification.Create(emailNotification);

        return Created("", emailNotification);
    }

    [HttpPost("sms")]
    public async Task<ActionResult> CreateSMSNotification([FromBody] SMS smsNotification)
    {
        await _smsNotification.Create(smsNotification);

        return Created("", smsNotification);
    }
}
