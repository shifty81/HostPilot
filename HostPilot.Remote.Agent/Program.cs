using HostPilot.Remote.Agent.Handlers;
using HostPilot.Remote.Agent.Options;
using HostPilot.Remote.Agent.Services;
using HostPilot.Remote.Transport.Abstractions;
using HostPilot.Remote.Transport.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RemoteAgentOptions>(builder.Configuration.GetSection(RemoteAgentOptions.SectionName));
builder.Services.AddSingleton<INonceStore, InMemoryNonceStore>();
builder.Services.AddSingleton<IEnvelopeSigner>(_ => new DevelopmentEnvelopeSigner("replace-me"));
builder.Services.AddSingleton<IEnvelopeValidator, DevelopmentEnvelopeValidator>(_ =>
    new DevelopmentEnvelopeValidator("replace-me", _.GetRequiredService<INonceStore>()));
builder.Services.AddSingleton<AgentNodeIdentityProvider>();
builder.Services.AddSingleton<AgentHealthSampler>();
builder.Services.AddSingleton<AgentCommandRegistry>();
builder.Services.AddSingleton<IAgentCommandHandler, StartServerHandler>();
builder.Services.AddSingleton<IAgentCommandHandler, StopServerHandler>();
builder.Services.AddSingleton<IAgentCommandHandler, BackupServerHandler>();
builder.Services.AddSingleton<AgentCommandDispatcher>();
builder.Services.AddHostedService<AgentWorkerService>();

var app = builder.Build();
await app.RunAsync();
