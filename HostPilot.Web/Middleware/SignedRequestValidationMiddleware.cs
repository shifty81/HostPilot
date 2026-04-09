using System.Text.Json;
using HostPilot.Security.Contracts;
using HostPilot.Security.Services;

namespace HostPilot.Web.Middleware;

/// <summary>
/// Intercepts every request whose path starts with <c>/api/agent</c> and validates
/// that the request body carries a well-formed, non-replayed
/// <see cref="SignedRequestEnvelope"/>.  Requests that fail validation are rejected
/// with <c>400 Bad Request</c>.
/// </summary>
public sealed class SignedRequestValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SignedRequestValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, SignedRequestValidator validator)
    {
        if (!context.Request.Path.StartsWithSegments("/api/agent"))
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        var envelope = JsonSerializer.Deserialize<SignedRequestEnvelope>(body);
        if (envelope is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Invalid signed envelope.");
            return;
        }

        await validator.ValidateAsync(envelope, context.RequestAborted);
        await _next(context);
    }
}
