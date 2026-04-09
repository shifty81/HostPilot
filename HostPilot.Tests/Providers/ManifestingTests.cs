using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Models;
using HostPilot.Providers.Manifesting.Services;
using HostPilot.Providers.Manifesting.Enums;
using Xunit;

namespace HostPilot.Tests.Providers;

public sealed class ManifestConditionEvaluatorTests
{
    private readonly ManifestConditionEvaluator _evaluator = new();

    private static ManifestEvaluationContext MakeContext(ProviderManifest manifest, Dictionary<string, object?> state)
        => new() { Manifest = manifest, State = state };

    private static ProviderManifest EmptyManifest() => new()
    {
        Id = "test",
        Version = "1.0",
        SchemaVersion = "1.0",
        DisplayName = "Test",
        ProviderType = ProviderType.SteamCmd,
        GameFamily = "Test"
    };

    [Fact]
    public void Evaluate_NullGroup_ReturnsTrue()
    {
        var ctx = MakeContext(EmptyManifest(), []);
        Assert.True(_evaluator.Evaluate(null, ctx));
    }

    [Fact]
    public void Evaluate_EmptyGroup_ReturnsTrue()
    {
        var ctx = MakeContext(EmptyManifest(), []);
        var group = new ManifestConditionGroup();
        Assert.True(_evaluator.Evaluate(group, ctx));
    }

    [Fact]
    public void Evaluate_EqualsCondition_Match()
    {
        var ctx = MakeContext(EmptyManifest(), new Dictionary<string, object?> { ["mode"] = "pvp" });
        var group = new ManifestConditionGroup
        {
            Conditions = [new ManifestConditionDefinition { Path = "mode", Operator = ManifestConditionOperator.Equals, Value = "pvp" }]
        };
        Assert.True(_evaluator.Evaluate(group, ctx));
    }

    [Fact]
    public void Evaluate_EqualsCondition_NoMatch()
    {
        var ctx = MakeContext(EmptyManifest(), new Dictionary<string, object?> { ["mode"] = "pve" });
        var group = new ManifestConditionGroup
        {
            Conditions = [new ManifestConditionDefinition { Path = "mode", Operator = ManifestConditionOperator.Equals, Value = "pvp" }]
        };
        Assert.False(_evaluator.Evaluate(group, ctx));
    }

    [Fact]
    public void Evaluate_NotEmptyCondition_PopulatedValue()
    {
        var ctx = MakeContext(EmptyManifest(), new Dictionary<string, object?> { ["name"] = "MyServer" });
        var group = new ManifestConditionGroup
        {
            Conditions = [new ManifestConditionDefinition { Path = "name", Operator = ManifestConditionOperator.NotEmpty }]
        };
        Assert.True(_evaluator.Evaluate(group, ctx));
    }

    [Fact]
    public void Evaluate_OrLogic_AnyTrue()
    {
        var ctx = MakeContext(EmptyManifest(), new Dictionary<string, object?> { ["a"] = "1", ["b"] = "x" });
        var group = new ManifestConditionGroup
        {
            Logic = "Or",
            Conditions =
            [
                new ManifestConditionDefinition { Path = "a", Operator = ManifestConditionOperator.Equals, Value = "1" },
                new ManifestConditionDefinition { Path = "b", Operator = ManifestConditionOperator.Equals, Value = "nope" }
            ]
        };
        Assert.True(_evaluator.Evaluate(group, ctx));
    }
}

public sealed class ManifestDefaultResolverTests
{
    [Fact]
    public void BuildDefaultState_UsesFieldDefaults()
    {
        var manifest = new ProviderManifest
        {
            Id = "test",
            Version = "1.0",
            SchemaVersion = "1.0",
            DisplayName = "Test",
            ProviderType = ProviderType.SteamCmd,
            GameFamily = "Test",
            Fields =
            [
                new ManifestFieldDefinition { Id = "f1", SectionId = "s1", Key = "port", Type = ManifestFieldType.Port, Label = "Port", Default = 27015 }
            ]
        };

        var resolver = new ManifestDefaultResolver();
        var state = resolver.BuildDefaultState(manifest);

        Assert.Equal(27015, state["port"]);
    }

    [Fact]
    public void BuildDefaultState_PresetOverridesDefault()
    {
        var manifest = new ProviderManifest
        {
            Id = "test",
            Version = "1.0",
            SchemaVersion = "1.0",
            DisplayName = "Test",
            ProviderType = ProviderType.SteamCmd,
            GameFamily = "Test",
            Fields =
            [
                new ManifestFieldDefinition { Id = "f1", SectionId = "s1", Key = "port", Type = ManifestFieldType.Port, Label = "Port", Default = 27015 }
            ],
            Presets =
            [
                new ManifestPresetDefinition { Id = "default", Title = "Default", Overrides = new() { ["port"] = 7777 } }
            ]
        };

        var resolver = new ManifestDefaultResolver();
        var state = resolver.BuildDefaultState(manifest, presetId: "default");

        Assert.Equal(7777, state["port"]);
    }
}

public sealed class ManifestValidationEngineTests
{
    private static ProviderManifest MakeManifest(List<ManifestFieldDefinition> fields) => new()
    {
        Id = "test",
        Version = "1.0",
        SchemaVersion = "1.0",
        DisplayName = "Test",
        ProviderType = ProviderType.SteamCmd,
        GameFamily = "Test",
        Fields = fields
    };

    [Fact]
    public void Validate_RequiredFieldMissing_ReturnsError()
    {
        var manifest = MakeManifest(
        [
            new ManifestFieldDefinition { Id = "f1", SectionId = "s1", Key = "name", Type = ManifestFieldType.Text, Label = "Name", Required = true }
        ]);
        var engine = new ManifestValidationEngine(new ManifestConditionEvaluator());
        var result = engine.Validate(manifest, new Dictionary<string, object?> { ["name"] = null });
        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, i => i.Code == "Required");
    }

    [Fact]
    public void Validate_RequiredFieldPresent_NoErrors()
    {
        var manifest = MakeManifest(
        [
            new ManifestFieldDefinition { Id = "f1", SectionId = "s1", Key = "name", Type = ManifestFieldType.Text, Label = "Name", Required = true }
        ]);
        var engine = new ManifestValidationEngine(new ManifestConditionEvaluator());
        var result = engine.Validate(manifest, new Dictionary<string, object?> { ["name"] = "MyServer" });
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InvalidPort_ReturnsError()
    {
        var manifest = MakeManifest(
        [
            new ManifestFieldDefinition { Id = "f1", SectionId = "s1", Key = "port", Type = ManifestFieldType.Port, Label = "Port" }
        ]);
        var engine = new ManifestValidationEngine(new ManifestConditionEvaluator());
        var result = engine.Validate(manifest, new Dictionary<string, object?> { ["port"] = "99999" });
        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, i => i.Code == "PortRange");
    }
}

public sealed class ManifestOutputMapperTests
{
    [Fact]
    public void BuildOutput_LaunchArg_IncludedInList()
    {
        var manifest = new ProviderManifest
        {
            Id = "test",
            Version = "1.0",
            SchemaVersion = "1.0",
            DisplayName = "Test",
            ProviderType = ProviderType.SteamCmd,
            GameFamily = "Test",
            Fields =
            [
                new ManifestFieldDefinition
                {
                    Id = "f1",
                    SectionId = "s1",
                    Key = "port",
                    Type = ManifestFieldType.Port,
                    Label = "Port",
                    Output = new ManifestFieldOutputDefinition
                    {
                        Targets = [new ManifestOutputTargetDefinition { Type = ManifestOutputTargetType.LaunchArg, ArgName = "-port" }]
                    }
                }
            ]
        };

        var mapper = new ManifestOutputMapper();
        var output = mapper.BuildOutput(manifest, new Dictionary<string, object?> { ["port"] = 27015 });

        Assert.Contains(output.LaunchArguments, a => a.Contains("-port"));
    }
}
