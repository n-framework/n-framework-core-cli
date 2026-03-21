# Specification: Source Generator

**ID**: 003
**Title**: Source Generator
**Status**: Implemented
**Created**: 2025-03-19
**Updated**: 2025-03-19

## Overview

This specification defines the Roslyn source generator that produces Spectre.Console.Cli-compatible boilerplate from attribute-annotated classes. The generator eliminates manual command registration and produces adapter classes that bridge between Spectre.Console.Cli and the NFramework abstractions.

## User Scenarios & Testing

### Scenario 1: Generate Application Entry Point

**Given** a class marked with `[CliApplication]` that implements `ICliApplication`
**When** the project is built
**Then** a partial class should be generated with a `Create()` method
**And** a `GeneratedCliApplication` class should implement `Run(string[] args)`
**And** all commands should be registered with the command app

### Scenario 2: Generate Command Adapter

**Given** a class marked with `[CliCommand]` that implements `IAsyncCliCommand<TSettings>`
**When** the project is built
**Then** a settings class should be generated inheriting from `CommandSettings`
**And** an adapter class should be generated inheriting from `AsyncCommand<TSettings>`
**And** the adapter should delegate to the original command

### Scenario 3: Bind Arguments and Options

**Given** a settings type with properties marked with `[CliArgument]` or `[CliOption]`
**When** the generator creates the settings class
**Then** the properties should be generated with appropriate Spectre.Console attributes
**And** the adapter should copy values from Spectre settings to CLI settings

### Scenario 4: Include Examples in Help Text

**Given** a command marked with `[CliExample]` attributes
**When** the generator configures the command
**Then** the examples should be added to the command configurator
**And** they should appear in the help text

### Scenario 5: Validate Examples (Optional)

**Given** the application has `ValidateExamples = true`
**When** the command app is configured
**Then** `ValidateExamples()` should be called on the configurator
**And** invalid examples will be detected at runtime

### Scenario 6: Register Commands with DI

**Given** the application has commands
**When** `Create()` is called
**Then** all command types should be registered as singletons
**And** a `TypeRegistrar` should be created from the service collection

### Scenario 7: Handle Nested Namespaces

**Given** an application in a non-global namespace
**When** code is generated
**Then** the generated code should be in the same namespace
**And** the namespace should be correctly formatted

## Requirements

### FR-GEN-001: Incremental Generator Implementation

The generator must:

- Implement `IIncrementalGenerator` for performance
- Use `CreateSyntaxProvider` to detect attribute usage
- Separate application and command discovery pipelines
- Combine results in the output phase

**Acceptance Criteria:**

- [x] Generator class implements `IIncrementalGenerator`
- [x] `Initialize()` registers syntax providers and output callback
- [x] Application types are collected separately from commands
- [x] Both pipelines use incremental providers

### FR-GEN-002: Application Type Discovery

The generator must:

- Detect classes with `[CliApplication]` attribute
- Filter to only `ClassDeclarationSyntax` nodes with attributes
- Return the `INamedTypeSymbol` for further processing

**Acceptance Criteria:**

- [x] `getApplicationType()` returns null for non-class nodes
- [x] Returns null for classes without `[CliApplication]`
- [x] Returns the type symbol when attribute is present

### FR-GEN-003: Command Model Creation

The generator must:

- Detect classes with `[CliCommand]` attribute
- Extract the settings type from `IAsyncCliCommand<TSettings>`
- Extract command name and description from attribute
- Extract examples from `[CliExample]` attributes
- Extract properties from settings type with `[CliArgument]` or `[CliOption]`
- Validate settings type has a public parameterless constructor

**Acceptance Criteria:**

- [x] `getCommandModel()` returns null for non-command classes
- [x] Extracts settings type from generic interface
- [x] Validates command name and description are not empty
- [x] Collects all example attributes
- [x] Collects all non-static properties with CLI attributes
- [x] Returns null if settings type lacks public parameterless constructor

### FR-GEN-004: Property Model Creation

The generator must:

- Detect `[CliArgument]` or `[CliOption]` attributes
- Extract position, value name, and required flag for arguments
- Extract template, hidden, required, and optional flags for options
- Extract description from `[Description]` attribute
- Create appropriate model for attribute type

**Acceptance Criteria:**

- [x] `createPropertyModel()` returns null for properties without CLI attributes
- [x] Correctly extracts all argument properties
- [x] Correctly extracts all option properties
- [x] Returns null for properties without attributes

### FR-GEN-005: Application Template Rendering

The generator must:

- Render the application template with all commands
- Include namespace declaration if not global
- Generate `Create()` method that registers commands
- Generate `GeneratedCliApplication` class that implements `ICliApplication`
- Include example validation if enabled

**Acceptance Criteria:**

- [x] Template renders namespace conditionally
- [x] `Create()` method registers all commands as singletons
- [x] `GeneratedCliApplication` wraps Spectre.Console.Cli.CommandApp
- [x] `Run()` method delegates to command app
- [x] Example validation is conditionally included

### FR-GEN-006: Settings Template Rendering

The generator must:

- Generate a settings class inheriting from `CommandSettings`
- Include `[Description]` attributes if present
- Include `[CommandArgument]` attributes for arguments
- Include `[CommandOption]` attributes for options
- Generate properties with correct types and names

**Acceptance Criteria:**

- [x] Generated class inherits from `CommandSettings`
- [x] Properties are public with getters and setters
- [x] Attributes are correctly formatted
- [x] Property types match original settings type

### FR-GEN-007: Adapter Template Rendering

The generator must:

- Generate an adapter class inheriting from `AsyncCommand<TSettings>`
- Accept the original command in constructor
- Implement `ExecuteAsync()` that delegates to the command
- Create `CliCommandContext` from Spectre context
- Create CLI settings from Spectre settings
- Copy all property values from Spectre to CLI settings

**Acceptance Criteria:**

- [x] Adapter class is `file sealed`
- [x] Constructor stores the command
- [x] `ExecuteAsync()` creates correct context and settings
- [x] All property values are copied
- [x] Method delegates to `command.ExecuteAsync()`

### FR-GEN-008: DI Integration

The generator must:

- Register all commands as singletons in `Create()`
- Create `TypeRegistrar` from the service collection
- Pass the registrar to the command app

**Acceptance Criteria:**

- [x] Commands are registered via `AddSingleton<T>()`
- [x] `TypeRegistrar` wraps the service collection
- [x] Command app is created with the registrar

### FR-GEN-009: Source File Naming

The generator must:

- Name generated files with a `.CliApplication.g.cs` suffix
- Use the application type name as the base name

**Acceptance Criteria:**

- [x] `context.AddSource()` uses correct naming pattern
- [x] Generated files are marked as auto-generated

### FR-GEN-010: Example Handling

The generator must:

- Extract all example arguments from `[CliExample]` attributes
- Format examples as comma-separated literals
- Add examples to command configurators

**Acceptance Criteria:**

- [x] Examples are extracted from attribute constructor arguments
- [x] Example arguments are converted to C# string literals
- [x] Examples are added via `WithExample()`

## Success Criteria

### SC-GEN-001

Generated code compiles without warnings.

### SC-GEN-002

Generated code is AOT-compatible.

### SC-GEN-003

Generator completes in under 500ms for typical applications.

### SC-GEN-004

Generator produces no diagnostics for valid input.

### SC-GEN-005

Generated code is human-readable for debugging.

## Edge Cases

### EC-GEN-001

Commands with no arguments or options should generate empty settings classes.

### EC-GEN-002

Applications in the global namespace should not generate namespace declarations.

### EC-GEN-003

Settings types without public parameterless constructors should be ignored.

### EC-GEN-004

Properties without CLI attributes should be ignored.

### EC-GEN-005

Example attributes with empty arrays should be ignored.

### EC-GEN-006

Commands with duplicate names should generate a warning.

### EC-GEN-007

Argument position conflicts should be detected by Spectre.Console at runtime.

## Non-Goals

- This spec does not cover runtime command discovery
- This spec does not cover dynamic command registration
- This spec does not cover command aliases
- This spec does not cover middleware or pipelines
- This spec does not cover custom help text formatting

## Related Specifications

- [001: CLI Abstractions](./001-cli-abstractions.md) — Attribute definitions
- [002: Spectre.Console Integration](./002-spectre-console-integration.md) — UI implementation

## Implementation Notes

### Template Engine

The generator uses Scriban for template rendering. Templates are stored as constant strings in partial classes for organization.

### Symbol Display Format

The generator uses `FullyQualifiedTypeFormat` to ensure type names are globally qualified in generated code, avoiding ambiguity.

### Distinct Collection

Commands are deduplicated using `SymbolEqualityComparer.Default` before generation to avoid duplicate registrations.

### Type Registrar

The `TypeRegistrar` class (in the SpectreConsoleUI project) bridges between Microsoft.Extensions.DependencyInjection and Spectre.Console.Cli's DI abstraction.

### Command Context

The adapter creates a `CliCommandContext` with the command name (or default if empty) and remaining arguments from the Spectre context.

### Settings Mapping

Property values are copied by name from the generated Spectre settings to the original CLI settings. This requires properties to have the same names in both types.

## Generated Code Structure

For an application named `MyApp` with a `GreetCommand`:

```csharp
// Generated by source generator
partial class MyApp
{
    public static ICliApplication Create(IServiceCollection services)
    {
        // Register commands
        services.AddSingleton<GreetCommand>();

        // Create command app
        var commandApp = new CommandApp(new TypeRegistrar(services));
        commandApp.Configure(config =>
        {
            config.SetApplicationName("myapp");
            config.AddCommand<GreetCommandAdapter>("greet")
                .WithDescription("Prints a greeting");
        });

        return new GeneratedCliApplication(commandApp);
    }

    file sealed class GeneratedCliApplication : ICliApplication
    {
        public int Run(string[] args) => _commandApp.Run(args);
    }

    file sealed class GreetCommandSettings : CommandSettings
    {
        [CommandArgument(0, "name")]
        public string Name { get; set; }
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
                context.Name ?? "greet",
                new List<string>(context.Arguments)
            );
            var cliSettings = new CliSettings
            {
                Name = settings.Name
            };
            return command.ExecuteAsync(cliContext, cliSettings, ct);
        }
    }
}
```
