using HostPilot.Core.OperationEngine.Services;
using HostPilot.Core.Services.SteamCmd;
using Xunit;

namespace HostPilot.Tests.OperationEngine;

public class ServerLockServiceTests
{
    [Fact]
    public void TryAcquire_NewServer_ReturnsTrue()
    {
        var svc = new ServerLockService();
        Assert.True(svc.TryAcquire("server-1"));
    }

    [Fact]
    public void TryAcquire_AlreadyLocked_ReturnsFalse()
    {
        var svc = new ServerLockService();
        svc.TryAcquire("server-1");
        Assert.False(svc.TryAcquire("server-1"));
    }

    [Fact]
    public void Release_UnlocksServer()
    {
        var svc = new ServerLockService();
        svc.TryAcquire("server-1");
        svc.Release("server-1");
        Assert.True(svc.TryAcquire("server-1"));
    }

    [Fact]
    public void IsLocked_ReturnsTrueWhileLocked()
    {
        var svc = new ServerLockService();
        svc.TryAcquire("server-1");
        Assert.True(svc.IsLocked("server-1"));
        svc.Release("server-1");
        Assert.False(svc.IsLocked("server-1"));
    }

    [Fact]
    public void DifferentServers_HaveIndependentLocks()
    {
        var svc = new ServerLockService();
        Assert.True(svc.TryAcquire("server-a"));
        Assert.True(svc.TryAcquire("server-b"));
        Assert.False(svc.TryAcquire("server-a"));
    }
}

public class ServerOperationCommandBuilderTests
{
    [Fact]
    public void BuildInstall_SetsCorrectType()
    {
        var builder = new ServerOperationCommandBuilder();
        var req = builder.BuildInstall("MyServer");
        Assert.Equal("INSTALL_SERVER", req.Type);
        Assert.Equal("MyServer", req.TargetId);
    }

    [Fact]
    public void BuildUpdate_SetsCorrectType()
    {
        var builder = new ServerOperationCommandBuilder();
        var req = builder.BuildUpdate("MyServer");
        Assert.Equal("UPDATE_SERVER", req.Type);
    }

    [Fact]
    public void BuildRestart_SetsCorrectType()
    {
        var builder = new ServerOperationCommandBuilder();
        var req = builder.BuildRestart("MyServer");
        Assert.Equal("RESTART_SERVER", req.Type);
    }

    [Fact]
    public void BuildInstallMods_SetsCorrectType()
    {
        var builder = new ServerOperationCommandBuilder();
        var req = builder.BuildInstallMods("MyServer");
        Assert.Equal("INSTALL_MODS", req.Type);
    }

    [Fact]
    public void BuildImportExisting_SetsCorrectType()
    {
        var builder = new ServerOperationCommandBuilder();
        var req = builder.BuildImportExisting("MyServer");
        Assert.Equal("IMPORT_EXISTING_SERVER", req.Type);
    }
}

public class SteamCmdOutputParserTests
{
    private readonly SteamCmdOutputParser _parser = new();

    [Fact]
    public void Parse_SuccessLine_ReturnsSuccessType()
    {
        var result = _parser.Parse("Success!");
        Assert.Equal(SteamCmdLineType.Success, result.LineType);
        Assert.True(result.IsTerminalSuccess);
    }

    [Fact]
    public void Parse_ErrorLine_ReturnsErrorType()
    {
        var result = _parser.Parse("Error! App not found.");
        Assert.Equal(SteamCmdLineType.Error, result.LineType);
        Assert.True(result.IsTerminalFailure);
    }

    [Fact]
    public void Parse_ProgressLine_ExtractsPercent()
    {
        var result = _parser.Parse("Downloading Update (55%)");
        Assert.Equal(SteamCmdLineType.Progress, result.LineType);
        Assert.Equal(55, result.Percent);
    }

    [Fact]
    public void Parse_EmptyLine_ReturnsUnknown()
    {
        var result = _parser.Parse("   ");
        Assert.Equal(SteamCmdLineType.Unknown, result.LineType);
    }

    [Fact]
    public void Parse_NullLine_DoesNotThrow()
    {
        var result = _parser.Parse(null);
        Assert.NotNull(result);
    }
}

public class SteamCmdArgumentBuilderTests
{
    private readonly SteamCmdArgumentBuilder _builder = new();

    [Fact]
    public void Build_InstallJob_ContainsAppUpdate()
    {
        var profile = new SteamCmdProfile
        {
            SteamCmdPath = @"C:\steamcmd\steamcmd.exe",
            InstallDirectory = @"C:\Servers\ARK",
            AppId = "2430930",
        };

        var args = _builder.Build(profile, SteamCmdJobKind.Install);
        Assert.Contains("+app_update 2430930", args);
        Assert.Contains("+force_install_dir", args);
        Assert.Contains("+quit", args);
    }

    [Fact]
    public void Build_ValidateJob_ContainsValidate()
    {
        var profile = new SteamCmdProfile { AppId = "896660" };
        var args = _builder.Build(profile, SteamCmdJobKind.Validate);
        Assert.Contains("validate", args);
    }

    [Fact]
    public void Build_NonPublicBranch_ContainsBetaFlag()
    {
        var profile = new SteamCmdProfile { AppId = "123", Branch = "experimental" };
        var args = _builder.Build(profile, SteamCmdJobKind.Install);
        Assert.Contains("-beta experimental", args);
    }

    [Fact]
    public void Build_PublicBranch_DoesNotContainBetaFlag()
    {
        var profile = new SteamCmdProfile { AppId = "123", Branch = "public" };
        var args = _builder.Build(profile, SteamCmdJobKind.Install);
        Assert.DoesNotContain("-beta", args);
    }
}
