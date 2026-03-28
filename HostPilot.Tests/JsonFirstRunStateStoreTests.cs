using HostPilot.Core.Models.FirstRun;
using HostPilot.Core.Services.Startup;

namespace HostPilot.Tests;

public sealed class JsonFirstRunStateStoreTests
{
    [Fact]
    public void Save_ThenLoad_RoundTripsWizardState()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "HostPilotTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        var statePath = Path.Combine(tempRoot, "first-run-state.json");

        try
        {
            var store = new JsonFirstRunStateStore(statePath);
            var expected = new FirstRunWizardState
            {
                HasCompletedWizard = true,
                HasConfiguredSteamCmd = true,
                HasCompletedDiscoveryScan = true,
                ImportedCandidateIds = new List<string> { "a", "b" }
            };

            store.Save(expected);
            var actual = store.Load();

            Assert.True(actual.HasCompletedWizard);
            Assert.True(actual.HasConfiguredSteamCmd);
            Assert.True(actual.HasCompletedDiscoveryScan);
            Assert.Equal(new[] { "a", "b" }, actual.ImportedCandidateIds);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }
}
