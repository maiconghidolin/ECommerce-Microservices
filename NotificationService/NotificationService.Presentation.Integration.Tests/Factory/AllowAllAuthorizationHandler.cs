using Microsoft.AspNetCore.Authorization;

namespace NotificationService.Presentation.Integration.Tests.Factory;

public class AllowAllAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var req in context.Requirements)
            context.Succeed(req);

        return Task.CompletedTask;
    }
}