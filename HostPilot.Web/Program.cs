using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Execution.Abstractions;
using HostPilot.Remote.Execution.Services;
using HostPilot.Remote.Files;
using HostPilot.Remote.Transfer.Services;
using HostPilot.Remote.Updates.Abstractions;
using HostPilot.Remote.Updates.Services;
using HostPilot.Runtime;
using HostPilot.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// Runtime
builder.Services.AddSingleton<IRemoteNodeRegistry, RemoteNodeRegistry>();
builder.Services.AddSingleton<IRemoteNodeLeaseService, RemoteNodeLeaseService>();
builder.Services.AddSingleton<IWebAuditService, InMemoryAuditService>();
builder.Services.AddSingleton<IRemoteNodeTransport, StubRemoteNodeTransport>();
builder.Services.AddSingleton<IRemoteCommandRouter, RemoteCommandRouter>();
builder.Services.AddSingleton<IAuthorizationPolicyService, AuthorizationPolicyService>();

// Node dashboard
builder.Services.AddSingleton<NodeDashboardStore>();

// Remote files
builder.Services.AddSingleton<RemoteFileBrowserService>();

// Remote execution
builder.Services.AddSingleton<RemoteCommandCatalog>();
builder.Services.AddSingleton<ICommandExecutionPolicy, DefaultCommandExecutionPolicy>();
builder.Services.AddSingleton<IRemoteCommandExecutor, DevelopmentRemoteCommandExecutor>();
builder.Services.AddSingleton<ICommandExecutionSink, NullCommandExecutionSink>();
builder.Services.AddSingleton<RemoteCommandExecutionService>();

// Remote transfer
builder.Services.AddSingleton<IRemoteFileTransferProvider, SftpTransferProvider>();
builder.Services.AddSingleton<FileTransferProgressBroadcaster>();
builder.Services.AddSingleton<RemoteFileTransferService>(sp =>
    new RemoteFileTransferService(
        sp.GetServices<IRemoteFileTransferProvider>(),
        sp.GetRequiredService<FileTransferProgressBroadcaster>()));

// Remote updates
builder.Services.AddSingleton<UpdateManifestValidator>();
builder.Services.AddSingleton<InMemoryUpdateStateStore>();
builder.Services.AddSingleton<IRemoteUpdatePackageSource, NullRemoteUpdatePackageSource>();
builder.Services.AddSingleton<IRemoteUpdateCoordinator, RemoteUpdateCoordinator>();

// Facade + rollout
builder.Services.AddSingleton<RemoteOperationsFacade>();
builder.Services.AddSingleton<RolloutCoordinator>();

builder.Services.AddAuthentication("Cookies").AddCookie("Cookies");
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RemoteControl", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapHub<RuntimeEventsHub>("/hubs/runtime-events");
app.MapGet("/api/nodes/all", (NodeDashboardStore store) => Results.Ok(store.GetNodes()))
   .RequireAuthorization("RemoteControl");

app.Run();
