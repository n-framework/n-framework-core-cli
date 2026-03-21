using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators.Tests;

public class GeneratorIntegrationTests
{
    [Fact]
    public async Task GeneratorProducesExpectedOutput_ForApplicationWithCommand()
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

        // Act & Assert
        await VerifyGeneratorOutput(testCode);
    }

    [Fact]
    public async Task GeneratorProducesExpectedOutput_ForApplicationWithMultipleCommands()
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
[CliExample(""test --name John"")]
public class GreetCommand : IAsyncCliCommand<GreetSettings>
{
    public Task<int> ExecuteAsync(CliCommandContext context, GreetSettings settings, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

[CliCommand]
[CliExample(""list --all"")]
[CliExample(""list --active"")]
public class ListCommand : IAsyncCliCommand<ListSettings>
{
    public Task<int> ExecuteAsync(CliCommandContext context, ListSettings settings, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

public class GreetSettings
{
    [CliArgument(0, ""name"")]
    public string? Name { get; set; }
}

public class ListSettings
{
    [CliOption(""--all"")]
    public bool All { get; set; }

    [CliOption(""--active"")]
    public bool Active { get; set; }
}";

        // Act & Assert
        await VerifyGeneratorOutput(testCode);
    }

    [Fact]
    public async Task GeneratorProducesExpectedOutput_ForCommandWithComplexSettings()
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
public class ComplexCommand : IAsyncCliCommand<ComplexSettings>
{
    public Task<int> ExecuteAsync(CliCommandContext context, ComplexSettings settings, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

public class ComplexSettings
{
    [CliArgument(0, ""input"")]
    public string Input { get; set; } = string.Empty;

    [CliArgument(1, ""output"")]
    public string Output { get; set; } = string.Empty;

    [CliOption(""--timeout"")]
    [Description(""Timeout in seconds"")]
    public int Timeout { get; set; } = 30;

    [CliOption(""--verbose"")]
    public bool Verbose { get; set; }

    [CliOption(""--dry-run"")]
    [Description(""Perform a dry run without actual execution"")]
    public bool DryRun { get; set; }
}";

        // Act & Assert
        await VerifyGeneratorOutput(testCode);
    }

    private static async Task VerifyGeneratorOutput(string sourceCode)
    {
        // Create a C# compilation with the test source
        var compilation = CreateCompilation(sourceCode);

        // Get the generator from the compilation's references
        CliSpectreApplicationGenerator generator = GetGenerator(compilation);

        // Set up the generator test
        var driver = new CSharpSourceGeneratorDriver(
            generator,
            additionalTexts: Enumerable.Empty<AdditionalText>(),
            parseOptions: compilation.SyntaxTrees.First().Options,
            optionsProvider: null,
            reporter: new TestOutputReporter()
        );

        // Run the generator
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Verify there are no diagnostics
        Assert.Empty(diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning));

        // Check if generation succeeded by looking for generated files
        var generatedFiles = outputCompilation
            .SyntaxTrees.Where(t => t.FilePath.EndsWith(".CliApplication.g.cs"))
            .ToList();

        Assert.NotEmpty(generatedFiles);

        // Basic verification that generated files contain expected content
        foreach (var tree in generatedFiles)
        {
            var text = tree.GetText().ToString();
            Assert.Contains("GeneratedCliApplication", text);
            Assert.Contains("TypeRegistrar", text);
            Assert.Contains("CommandApp", text);
        }
    }

    private static CSharpCompilation CreateCompilation(string sourceCode)
    {
        return CSharpCompilation.Create(
            "TestAssembly",
            new[] { CSharpSyntaxTree.ParseText(sourceCode) },
            new[]
            {
                // Add references to the necessary assemblies
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICliApplication).Assembly.Location),
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
    }

    private static CliSpectreApplicationGenerator GetGenerator(CSharpCompilation compilation)
    {
        // This is a simplified approach. In a real scenario, you'd need to
        // properly instantiate the generator and set up its dependencies
        return new CliSpectreApplicationGenerator();
    }

    private class TestOutputReporter : ISourceGeneratorTestReporter
    {
        public void ReportDiagnostic(Diagnostic diagnostic)
        {
            // In a real test, you might want to report diagnostics
            // For now, we'll just ignore them in this example
        }
    }
}
