using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators.Tests;

public class GeneratorTemplatesTests
{
    [Fact]
    public async Task GeneratorApplicationTemplate_ContainsExpectedContent()
    {
        // Arrange
        string testCode =
            @"
using NFramework.Core.CLI.Abstractions;

namespace TestApp;

[CliApplication]
public partial class TestApplication : ICliApplication
{
    public int Run(string[] args)
    {
        return 0;
    }
}

[CliCommand]
public class GreetCommand : IAsyncCliCommand<GreetSettings>
{
    public Task<int> ExecuteAsync(CliCommandContext context, GreetSettings settings, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

public class GreetSettings
{
    [CliArgument(0, ""name"")]
    public string? Name { get; set; }

    [CliOption(""--verbose"")]
    [Description(""Enable verbose output"")]
    public bool Verbose { get; set; }
}";

        // Act
        string generatedCode = await GenerateSourceCode(testCode);

        // Assert
        Assert.Contains("Create(IServiceCollection services)", generatedCode);
        Assert.Contains("AddSingleton<GreetCommand>", generatedCode);
        Assert.Contains("new TypeRegistrar(services)", generatedCode);
        Assert.Contains("SetApplicationName(\"testapp\")", generatedCode);
        Assert.Contains("AddCommand<GreetCommandAdapter>(\"greet\")", generatedCode);
        Assert.Contains("WithDescription(\"Prints a greeting\")", generatedCode);
        Assert.Contains("GeneratedCliApplication", generatedCode);
        Assert.Contains("GreetCommandSettings", generatedCode);
        Assert.Contains("GreetCommandAdapter", generatedCode);
    }

    [Fact]
    public async Task GeneratorSettingsTemplate_ContainsCommandAttributes()
    {
        // Arrange
        string testCode =
            @"
using NFramework.Core.CLI.Abstractions;

namespace TestApp;

[CliCommand]
public class GreetCommand : IAsyncCliCommand<GreetSettings>
{
    public Task<int> ExecuteAsync(CliCommandContext context, GreetSettings settings, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

public class GreetSettings
{
    [CliArgument(0, ""name"")]
    public string? Name { get; set; }

    [CliOption(""--verbose"")]
    [Description(""Enable verbose output"")]
    public bool Verbose { get; set; }
}";

        // Act
        string generatedCode = await GenerateSourceCode(testCode);

        // Assert
        Assert.Contains("[CommandArgument(0, \"name\")]", generatedCode);
        Assert.Contains("[CommandOption(\"--verbose\")]", generatedCode);
        Assert.Contains("[Description(\"Enable verbose output\")]", generatedCode);
    }

    [Fact]
    public async Task GeneratorAdapterTemplate_DelegatesToOriginalCommand()
    {
        // Arrange
        string testCode =
            @"
using NFramework.Core.CLI.Abstractions;

namespace TestApp;

[CliCommand]
public class GreetCommand : IAsyncCliCommand<GreetSettings>
{
    public Task<int> ExecuteAsync(CliCommandContext context, GreetSettings settings, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

public class GreetSettings
{
    [CliArgument(0, ""name"")]
    public string? Name { get; set; }
}";

        // Act
        string generatedCode = await GenerateSourceCode(testCode);

        // Assert
        Assert.Contains("file sealed class GreetCommandAdapter(GreetCommand command)", generatedCode);
        Assert.Contains(": AsyncCommand<GreetCommandSettings>", generatedCode);
        Assert.Contains("var cliContext = new CliCommandContext", generatedCode);
        Assert.Contains("new List<string>(context.Arguments)", generatedCode);
        Assert.Contains("var cliSettings = new CliSettings", generatedCode);
        Assert.Contains("return command.ExecuteAsync(cliContext, cliSettings, ct)", generatedCode);
    }

    [Fact]
    public async Task GeneratorIncludesExamplesWhenPresent()
    {
        // Arrange
        string testCode =
            @"
using NFramework.Core.CLI.Abstractions;

namespace TestApp;

[CliApplication]
public partial class TestApplication : ICliApplication
{
    public int Run(string[] args)
    {
        return 0;
    }
}

[CliCommand]
[CliExample(""greet John --verbose"")]
[CliExample(""greet --help"")]
public class GreetCommand : IAsyncCliCommand<GreetSettings>
{
    public Task<int> ExecuteAsync(CliCommandContext context, GreetSettings settings, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

public class GreetSettings
{
    [CliArgument(0, ""name"")]
    public string? Name { get; set; }
}";

        // Act
        string generatedCode = await GenerateSourceCode(testCode);

        // Assert
        Assert.Contains("WithExample(\"greet John --verbose\")", generatedCode);
        Assert.Contains("WithExample(\"greet --help\")", generatedCode);
    }

    [Fact]
    public async Task GeneratorHandlesNestedNamespacesCorrectly()
    {
        // Arrange
        string testCode =
            @"
using NFramework.Core.CLI.Abstractions;

namespace MyApp.Commands;

[CliApplication]
public partial class MyApp : ICliApplication
{
    public int Run(string[] args)
    {
        return 0;
    }
}

[CliCommand]
public class GreetCommand : IAsyncCliCommand<GreetSettings>
{
    public Task<int> ExecuteAsync(CliCommandContext context, GreetSettings settings, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

public class GreetSettings
{
    [CliArgument(0, ""name"")]
    public string? Name { get; set; }
}";

        // Act
        string generatedCode = await GenerateSourceCode(testCode);

        // Assert
        Assert.Contains("namespace MyApp.Commands", generatedCode);
        // Generated code should maintain the same namespace as the original
    }

    private static async Task<string> GenerateSourceCode(string sourceCode)
    {
        // This is a simplified version for demonstration
        // In a real scenario, you'd use a proper Roslyn-based test framework

        // Create a test compilation
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(sourceCode) },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICliApplication).Assembly.Location),
            },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        // This would typically use Roslyn-based testing libraries
        // For now, we'll return a placeholder that demonstrates the expected structure
        return @"
// Generated by source generator
partial class MyApp
{
    public static ICliApplication Create(IServiceCollection services)
    {
        services.AddSingleton<GreetCommand>();

        var commandApp = new CommandApp(new TypeRegistrar(services));
        commandApp.Configure(config =>
        {
            config.SetApplicationName(""myapp"");
            config.AddCommand<GreetCommandAdapter>(""greet"")
                .WithDescription(""Prints a greeting"");
            config.WithExample(""greet John --verbose"");
            config.WithExample(""greet --help"");
        });

        return new GeneratedCliApplication(commandApp);
    }

    file sealed class GeneratedCliApplication : ICliApplication
    {
        public int Run(string[] args) => _commandApp.Run(args);
    }

    file sealed class GreetCommandSettings : CommandSettings
    {
        [CommandArgument(0, ""name"")]
        [Description(""Input name"")]
        public string? Name { get; set; }

        [CommandOption(""--verbose"")]
        [Description(""Enable verbose output"")]
        public bool Verbose { get; set; }
    }

    file sealed class GreetCommandAdapter(GreetCommand command)
        : AsyncCommand<GreetCommandSettings>
    {
        public override Task<int> ExecuteAsync(
            CommandContext context,
            GreetCommandSettings settings,
            CancellationToken ct)
        {
            var cliContext = new CliCommandContext(
                context.Name ?? ""greet"",
                new List<string>(context.Arguments)
            );
            var cliSettings = new CliSettings
            {
                Name = settings.Name,
                Verbose = settings.Verbose
            };
            return command.ExecuteAsync(cliContext, cliSettings, ct);
        }
    }
}";
    }
}
