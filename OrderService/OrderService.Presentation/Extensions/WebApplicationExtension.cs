using Asp.Versioning.ApiExplorer;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using OrderService.Presentation.Middleware;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrderService.Presentation.Extensions;

public static class WebApplicationExtension
{
    public static void Configure(this WebApplication app, IConfiguration configuration)
    {
        var basePath = configuration["ApiPathBase"]?.Trim().TrimStart('/');

        if (!string.IsNullOrWhiteSpace(basePath))
            app.UsePathBase('/' + basePath);

        app.UseHealthChecks("/health", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseExceptionMiddleware();

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });

        app.UseSerilogRequestLogging();

        app.Use(async (context, next) =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader["Bearer ".Length..];
                var handler = new JwtSecurityTokenHandler();

                try
                {
                    var jwt = handler.ReadJwtToken(token);

                    var realmRolesClaim = jwt.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
                    var claims = new List<Claim>();

                    Log.Logger.Information($"realmRolesClaim: {realmRolesClaim}");

                    if (!string.IsNullOrEmpty(realmRolesClaim))
                    {
                        var roles = ((JObject)JToken.Parse(realmRolesClaim))["roles"];
                        if (roles != null)
                        {
                            foreach (var role in roles)
                            {
                                Log.Logger.Information($"role: {role}");
                                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                            }
                        }
                    }

                    var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                    if (!string.IsNullOrEmpty(nameClaim))
                    {
                        claims.Add(new Claim(ClaimTypes.Name, nameClaim));
                    }

                    var identity = new ClaimsIdentity(claims, "CustomJwt");
                    context.User = new ClaimsPrincipal(identity);
                }
                catch
                {
                    // Invalid token format → ignore or handle as needed
                }
            }

            await next();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });

        app.MapPrometheusScrapingEndpoint();
    }
}
