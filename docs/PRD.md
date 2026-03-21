# Product Requirements Document: NFramework Core-CLI Package

## Executive Summary

The NFramework Core-CLI package provides a compile-time-first CLI abstraction layer for .NET 11 applications. It enables developers to build rich command-line interfaces using attribute-based configuration while maintaining clean architecture boundaries and supporting Native AOT compilation. The package leverages Roslyn source generators to eliminate runtime reflection and create Spectre.Console-compatible boilerplate code automatically.

## 1. Introduction

### 1.1 Purpose

The Core-CLI package serves as the foundational layer for building command-line applications within the NFramework ecosystem. It provides:

- **Clean Architecture Support**: Abstractions that enable testable and maintainable CLI applications
- **Spectre.Console Integration**: Rich UI capabilities including prompts, selections, and tables
- **Compile-Time Generation**: Source generators that eliminate boilerplate and enable Native AOT
- **Attribute-Based Configuration**: Declarative command and option definition using custom attributes

### 1.2 Scope

**In Scope**:

- CLI application and command abstractions
- Spectre.Console UI integration
- Source generator for code generation
- Dependency injection extensions
- Unit testing utilities

**Out of Scope**:

- Runtime command discovery
- Dynamic command registration
- Custom help text generation
- Authentication and security features
- Internationalization support

### 1.3 Target Audience

- **Application Developers**: Building command-line tools with rich UI
- **Framework Developers**: Extending the CLI framework for custom use cases
- **DevOps Teams**: Creating deployment and automation CLIs
- **Enterprise Users**: Building maintainable CLI applications with clean architecture

## 2. User Scenarios

### 2.1 Scenario 1: Creating a Simple CLI Application

**As a** developer,
**I want to** create a simple CLI application with a greeting command,
**So that** users can get personalized greetings from the command line.

**Given** I want to create a CLI application
**When** I define a class with `[CliApplication]` attribute
**Then** the source generator should create a partial class with `Create()` method
**And** register all commands automatically

```csharp
[CliApplication]
public partial class MyApp : ICliApplication
{
    public int Run(string[] args)
    {
        return 0; // Implementation provided by generator
    }
}

[CliCommand]
public class GreetCommand : IAsyncCliCommand<GreetSettings>
{
    public Task<int> ExecuteAsync(CliCommandContext context, GreetSettings settings, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Hello, {settings.Name}!");
        return Task.FromResult(0);
    }
}

public class GreetSettings
{
    [CliArgument(0, "name")]
    public string? Name { get; set; }
}
```

### 2.2 Scenario 2: Interactive CLI with Prompts

**As a** developer,
**I want** to create interactive CLI applications with text and selection prompts,
**So that** users can provide input through rich terminal interfaces.

**Given** I need user input
**When** I use `ITerminalSession.PromptForTextAsync()`
**Then** a text prompt should be displayed with validation
**And** the result should contain the user input or cancellation

```csharp
public async Task<int> ExecuteAsync(CliCommandContext context, InteractiveSettings settings, CancellationToken cancellationToken)
{
    var terminal = context.Terminal;
    var namePrompt = new TerminalTextPrompt("Enter your name:");
    var nameResult = await terminal.PromptForTextAsync(namePrompt, cancellationToken);

    if (nameResult.WasCancelled)
        return 1;

    var selectionPrompt = new TerminalSelectionPrompt("Choose color:",
        new[] { new TerminalSelectionOption("red", "Red"), new TerminalSelectionOption("blue", "Blue") });
    var colorResult = await terminal.PromptForSelectionAsync(selectionPrompt, cancellationToken);

    if (colorResult.WasCancelled)
        return 1;

    Console.WriteLine($"{nameResult.Value} likes {colorResult.SelectedValue}!");
    return 0;
}
```

### 2.3 Scenario 3: Data Display with Tables

**As a** developer,
**I want** to display tabular data in a formatted table,
**So that** users can view structured information clearly.

**Given** I have tabular data to display
**When** I create a `TerminalTable` and call `RenderTable()`
**Then** the data should be displayed with borders and proper alignment

```csharp
public void DisplayUsers(List<User> users)
{
    var table = new TerminalTable(new[] { "ID", "Name", "Email" });
    foreach (var user in users)
    {
        table.AddRow(user.Id.ToString(), user.Name, user.Email);
    }
    _terminal.RenderTable(table);
}
```

## 3. Functional Requirements

### 3.1 CLI Abstractions Layer

#### FR-CLI-001: Application Interface

**Requirement**: The package must define `ICliApplication` interface as the entry point for CLI applications.

```csharp
public interface ICliApplication
{
    int Run(string[] args);
}
```

**Acceptance Criteria**:

- Interface defines single `Run` method
- Returns exit code (0 for success, non-zero for failure)
- Must be implemented by classes with `[CliApplication]` attribute

#### FR-CLI-002: Command Interface

**Requirement**: The package must define `IAsyncCliCommand<TSettings>` interface for command implementations.

```csharp
public interface IAsyncCliCommand<TSettings>
{
    Task<int> ExecuteAsync(CliCommandContext context, TSettings settings, CancellationToken cancellationToken);
}
```

**Acceptance Criteria**:

- Supports generic settings type
- Returns Task<int> for async execution
- Receives context and settings
- Supports cancellation

#### FR-CLI-003: Command Context

**Requirement**: The package must define `CliCommandContext` containing execution context.

```csharp
public readonly record struct CliCommandContext
{
    public string CommandName { get; }
    public IReadOnlyList<string> RemainingArguments { get; }
    public ITerminalSession Terminal { get; }
}
```

**Acceptance Criteria**:

- Contains command name (or default if empty)
- Contains remaining arguments after parsing
- Provides access to terminal session
- Is a readonly record struct for immutability

#### FR-CLI-004: Terminal Session Interface

**Requirement**: The package must define `ITerminalSession` for UI interactions.

```csharp
public interface ITerminalSession
{
    bool IsInteractive { get; }
    Task<TerminalTextInputResult> PromptForTextAsync(TerminalTextPrompt prompt, CancellationToken cancellationToken);
    Task<TerminalSelectionResult> PromptForSelectionAsync(TerminalSelectionPrompt prompt, CancellationToken cancellationToken);
    void WriteLine(string message);
    void WriteErrorLine(string message);
    void RenderTable(TerminalTable table);
}
```

**Acceptance Criteria**:

- Supports interactive detection
- Provides text and selection prompts
- Supports normal and error output
- Supports table rendering
- All methods handle cancellation gracefully

### 3.2 Spectre.Console Integration

#### FR-SPECTRE-001: Terminal Session Implementation

**Requirement**: The package must provide `SpectreConsoleTerminalSession` implementing `ITerminalSession`.

**Acceptance Criteria**:

- Uses Spectre.Console for rich UI
- Handles cancellation through OperationCanceledException
- Separates normal output from error output
- Supports non-interactive mode detection

#### FR-SPECTRE-002: Dependency Injection

**Requirement**: The package must provide `AddCoreCliSpectreConsoleUi()` extension method.

**Acceptance Criteria**:

- Registers `IAnsiConsole` as singleton
- Registers `ITerminalSession` as singleton
- Returns service collection for chaining
- Uses factory for console instance

### 3.3 Source Generator

#### FR-GEN-001: Application Generation

**Requirement**: The source generator must generate application entry points.

**Acceptance Criteria**:

- Generates partial class with `Create(IServiceCollection)` method
- Registers commands as singletons
- Creates Spectre.Console.CommandApp with TypeRegistrar
- Generates `GeneratedCliApplication` class

#### FR-GEN-002: Command Generation

**Requirement**: The source generator must generate command adapters.

**Acceptance Criteria**:

- Generates settings class inheriting from CommandSettings
- Generates adapter class inheriting from AsyncCommand<TSettings>
- Maps properties from Spectre settings to CLI settings
- Handles command context creation

#### FR-GEN-003: Attribute Processing

**Requirement**: The source generator must process CLI attributes.

**Acceptance Criteria**:

- Processes `[CliApplication]` on application classes
- Processes `[CliCommand]` on command classes
- Processes `[CliArgument]` and `[CliOption]` on settings properties
- Processes `[CliExample]` for command examples
- Processes `[Description]` for help text

## 4. Non-Functional Requirements

### 4.1 Performance

#### NFR-PERF-001: Native AOT Compatibility

**Requirement**: All generated code must be compatible with Native AOT compilation.

**Acceptance Criteria**:

- No reflection usage in generated code
- No dependency on missing trimmer assemblies
- Source generators compile with Native AOT
- Generated code passes AOT validation

#### NFR-PERF-002: Generator Performance

**Requirement**: Source generator must complete within 500ms for typical applications.

**Acceptance Criteria**:

- Incremental generation for subsequent builds
- Efficient symbol resolution
- Minimal memory usage
- No blocking compilation for large codebases

### 4.2 Security

#### NFR-SEC-001: Input Validation

**Requirement**: All user input must be validated before processing.

**Acceptance Criteria**:

- Arguments and options validated according to their types
- Empty/null inputs rejected with appropriate errors
- File system paths sanitized if used
- Command injection prevention in argument values

#### NFR-SEC-002: Cancellation Handling

**Requirement**: The system must handle cancellation gracefully.

**Acceptance Criteria**:

- Ctrl+C properly caught and handled
- No resource leaks on cancellation
- Clean shutdown of pending operations
- Proper state cleanup

### 4.3 Maintainability

#### NFR-MAIN-001: Clean Architecture

**Requirement**: Package must adhere to clean architecture principles.

**Acceptance Criteria**:

- Abstractions layer has no infrastructure dependencies
- Infrastructure layer implements abstractions
- No circular dependencies
- Clear separation of concerns

#### NFR-MAIN-002: Testability

**Requirement**: All components must be easily testable.

**Acceptance Criteria**:

- Dependency injection used throughout
- Mock-friendly interfaces
- No static dependencies
- Pure functions where possible

## 5. Quality Attributes

### 5.1 Reliability

- **Robust Error Handling**: All operations handle failures gracefully
- **Consistent Behavior**: Same inputs always produce same outputs
- **State Consistency**: No corrupted state on failures
- **Resource Management**: Proper disposal of all resources

### 5.2 Usability

- **Intuitive API**: Easy to understand and use
- **Rich Feedback**: Clear error messages and help text
- **Progressive Disclosure**: Advanced features available but not required
- \*\*Good Defaults Sensible defaults for common use cases

### 5.3 Extensibility

- **Plugin Architecture**: Easy to extend with new commands
- **Custom UI Components**: Ability to add custom terminal UI
- **Custom Generators**: Extensible source generator system
- **Hook Points**: Lifecycle hooks for customization

## 6. Implementation Constraints

### 6.1 Technical Constraints

- **.NET 11**: Must target .NET 11 with minimal APIs
- **C# 11**: Use modern C# features where applicable
- **Spectre.Console**: Version-compatible with Spectre.Console.Cli
- **Roslyn**: Use incremental generators for compilation

### 6.2 Dependency Constraints

- **No Reflection**: Runtime reflection prohibited in generated code
- **AOT Compatibility**: All dependencies must support Native AOT
- **Framework Only**: No application-specific dependencies

## 7. Success Criteria

### 7.1 Functional Criteria

- [ ] All abstractions interfaces are implemented
- [ ] Source generator produces correct output for all scenarios
- [ ] Spectre.Console integration works correctly
- [ ] Dependency injection works as expected
- [ ] All unit tests pass

### 7.2 Performance Criteria

- [ ] Source generator completes in <500ms
- [ ] CLI startup time <100ms
- [ ] Memory usage <10MB for typical applications
- [ ] Native AOT compilation succeeds

### 7.3 Quality Criteria

- [ ] 90%+ test coverage
- [ ] Zero breaking changes in public APIs
- [ ] All documented examples work
- [ ] Package size <100KB

## 8. Risks and Mitigation

### 8.1 Technical Risks

| Risk                                       | Probability | Impact | Mitigation                              |
| ------------------------------------------ | ----------- | ------ | --------------------------------------- |
| Source generator complexity                | Medium      | High   | Incremental development with testing    |
| Native AOT compatibility issues            | Low         | High   | Early and frequent testing with AOT     |
| Performance with large command sets        | Medium      | Medium | Efficient symbol resolution and caching |
| Version compatibility with Spectre.Console | Low         | Medium | Strict dependency versioning            |

### 8.2 Delivery Risks

| Risk                             | Probability | Impact | Mitigation                         |
| -------------------------------- | ----------- | ------ | ---------------------------------- |
| Documentation gaps               | Medium      | Medium | Early and continuous documentation |
| Learning curve for developers    | Medium      | Medium | Sample projects and tutorials      |
| Performance requirements not met | Low         | High   | Regular performance testing        |
| Feature creep                    | Low         | Medium | Strict scope management            |

## 9. Future Considerations

### 9.1 Potential Enhancements

- **Custom Templates**: Support for custom command templates
- **Plugin Commands**: Dynamic loading of command plugins
- **Advanced UI**: Additional Spectre.Console features (progress bars, spinners)
- **Configuration**: JSON/YAML configuration file support
- **Localization**: Multi-language support

### 9.2 Version Evolution

- **v1.1**: Enhanced error handling and diagnostics
- **v1.2**: Custom template support
- **v2.0**: Plugin architecture and advanced features

## 10. Appendix

### 10.1 Glossary

- **CLI**: Command Line Interface
- **AOT**: Ahead-of-Time compilation
- **DI**: Dependency Injection
- **Spectre.Console**: .NET library for rich console applications
- **Roslyn**: .NET compiler platform

### 10.2 References

- [Spectre.Console Documentation](https://spectreconsole.net/)
- [.NET Native AOT Documentation](https://learn.microsoft.com/dotnet/core/deploying/native-aot/)
- [Roslyn Source Generators](https://github.com/dotnet/roslyn/wiki/Source-generators-in-Roslyn-3.0)
