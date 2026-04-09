using HostPilot.Core.Execution.Contracts;
using HostPilot.Core.Execution.Services;
using Xunit;

namespace HostPilot.Tests.Execution;

public sealed class ExecutionPlanCompilerTests
{
    [Fact]
    public void Compile_Throws_When_Dependency_Is_Missing()
    {
        var compiler = new ExecutionPlanCompiler();
        var item = new ExecutionWorkItem(
            "install",
            "Install",
            "steam",
            "server-a",
            new ResourceRequirements(2, 2048, 10240, true, false, Array.Empty<string>()),
            Array.Empty<PlacementConstraint>(),
            new[] { "missing-step" },
            3,
            TimeSpan.FromMinutes(10),
            new Dictionary<string, string>());

        Assert.Throws<InvalidOperationException>(() =>
            compiler.Compile("plan-1", "Test Plan", "user", new[] { item }));
    }
}
