# Specification: CLI Abstractions

**ID**: 001
**Title**: CLI Abstractions
**Status**: Implemented
**Created**: 2025-03-19
**Updated**: 2025-03-19

## Overview

This specification defines the core abstractions for building CLI applications with NFramework.Core.CLI. These abstractions provide a clean separation between command definition, terminal interaction, and UI implementation.

## User Scenarios & Testing

### Scenario 1: Define a Simple CLI Application

**Given** a developer wants to create a CLI application
**When** they mark a class with `[CliApplication]` and implement `ICliApplication`
**Then** the source generator should provide the `Run()` implementation
**And** the application should execute commands defined in the assembly

### Scenario 2: Define a Command with Arguments

**Given** a command that accepts positional arguments
**When** the developer marks properties with `[CliArgument]`
**Then** the source generator should bind command-line arguments to those properties
**And** validation should ensure positions are unique and non-negative

### Scenario 3: Define a Command with Options

**Given** a command that accepts named options
**When** the developer marks properties with `[CliOption]`
**Then** the source generator should bind command-line options to those properties
**And** the template should follow Spectre.Console option syntax

### Scenario 4: Provide Help Text

**Given** a command or property
**When** the developer adds a description
**Then** the description should appear in the generated help text
**And** examples should be rendered in the usage section

### Scenario 5: Test Commands Without a Terminal

**Given** a command that uses terminal prompts
**When** the developer writes a unit test
**Then** they should be able to mock `ITerminalSession`
**And** the command should execute without requiring an actual terminal

## Requirements

### FR-ABSTRACTION-001: Application Entry Point

The system must provide an `ICliApplication` interface with a `Run(string[] args)` method that:

- Accepts command-line arguments
- Returns an integer exit code
- Is implemented via source generation for classes marked with `[CliApplication]`

**Acceptance Criteria:**

- [x] `ICliApplication` interface exists with `Run(string[] args)` method
- [x] Source generator generates partial class implementation
- [x] Generated code integrates with Spectre.Console.Cli

### FR-ABSTRACTION-002: Command Handler Interface

The system must provide an `IAsyncCliCommand<TSettings>` interface that:

- Defines an `ExecuteAsync` method accepting `CliCommandContext`, `TSettings`, and `CancellationToken`
- Returns `Task<int>` for the exit code
- Supports dependency injection through settings or constructor

**Acceptance Criteria:**

- [x] Interface exists with correct signature
- [x] Generic constraint allows custom settings types
- [x] Cancellation token is passed for cooperative cancellation

### FR-ABSTRACTION-003: Command Definition Attribute

The system must provide a `[CliCommand(name, description)]` attribute that:

- Marks classes as CLI commands
- Validates name is not empty or whitespace
- Validates description is not empty or whitespace
- Allows only one attribute per class

**Acceptance Criteria:**

- [x] Attribute exists with name and description parameters
- [x] Attribute has `AttributeTargets.Class` usage
- [x] Constructor validates both parameters
- [x] `AllowMultiple = false` prevents duplicate attributes

### FR-ABSTRACTION-004: Argument Binding Attribute

The system must provide a `[CliArgument(position, valueName)]` attribute that:

- Marks properties for positional argument binding
- Validates position is non-negative
- Validates valueName is not empty or whitespace
- Supports required argument specification

**Acceptance Criteria:**

- [x] Attribute exists with position and valueName parameters
- [x] Attribute has `AttributeTargets.Property` usage
- [x] `Position` property is read-only
- [x] `IsRequired` property can be set via init accessor

### FR-ABSTRACTION-005: Option Binding Attribute

The system must provide a `[CliOption(template)]` attribute that:

- Marks properties for named option binding
- Validates template is not empty or whitespace
- Supports hidden options
- Supports required options
- Supports options with optional values

**Acceptance Criteria:**

- [x] Attribute exists with template parameter
- [x] `IsHidden`, `IsRequired`, `ValueIsOptional` properties exist
- [x] All properties can be set via init accessors

### FR-ABSTRACTION-006: Example Documentation Attribute

The system must provide a `[CliExample(params string[])]` attribute that:

- Documents example usage for commands
- Validates at least one argument is provided
- Allows multiple examples per command

**Acceptance Criteria:**

- [x] Attribute exists with params string[] parameter
- [x] `AllowMultiple = true` permits multiple examples
- [x] Constructor validates arguments array is not empty

### FR-ABSTRACTION-007: Command Execution Context

The system must provide a `CliCommandContext` class that:

- Encapsulates the command name being executed
- Provides access to remaining arguments
- Validates inputs are not null or empty

**Acceptance Criteria:**

- [x] Class exists with `Name` and `Arguments` properties
- [x] Constructor validates name is not whitespace
- [x] Constructor validates arguments is not null
- [x] Properties are read-only

### FR-ABSTRACTION-008: Terminal Session Interface

The system must provide an `ITerminalSession` interface that:

- Abstracts terminal I/O operations
- Detects interactive mode
- Supports text input prompts
- Supports selection prompts
- Supports table rendering
- Supports error output

**Acceptance Criteria:**

- [x] Interface defines all required methods
- [x] `IsInteractive` property detects redirected I/O
- [x] All prompt methods accept `CancellationToken`
- [x] Result types distinguish between success and cancellation

### FR-ABSTRACTION-009: Text Input Result Type

The system must provide a `TerminalTextInputResult` type that:

- Distinguishes between submitted and cancelled outcomes
- Provides the submitted value when not cancelled
- Validates submitted values are not whitespace

**Acceptance Criteria:**

- [x] `Submitted(string)` factory method exists
- [x] `Cancelled()` factory method exists
- [x] `Value` property is nullable string
- [x] `WasCancelled` property indicates outcome
- [x] Factory validates input for `Submitted`

### FR-ABSTRACTION-010: Selection Result Type

The system must provide a `TerminalSelectionResult` type that:

- Distinguishes between selected and cancelled outcomes
- Provides the selected value when not cancelled
- Validates selected values are not whitespace

**Acceptance Criteria:**

- [x] `Selected(string)` factory method exists
- [x] `Cancelled()` factory method exists
- [x] `SelectedValue` property is nullable string
- [x] `WasCancelled` property indicates outcome
- [x] Factory validates input for `Selected`

### FR-ABSTRACTION-011: Text Prompt Type

The system must provide a `TerminalTextPrompt` type that:

- Defines prompt text
- Defines validation error message
- Validates both strings are not whitespace

**Acceptance Criteria:**

- [x] Constructor accepts prompt text and validation message
- [x] Both properties are read-only
- [x] Constructor validates both inputs

### FR-ABSTRACTION-012: Selection Prompt Type

The system must provide a `TerminalSelectionPrompt` type that:

- Defines prompt title
- Defines available options
- Validates title is not whitespace
- Validates at least one option is provided

**Acceptance Criteria:**

- [x] Constructor accepts title and options list
- [x] Both properties are read-only
- [x] Constructor validates both inputs

### FR-ABSTRACTION-013: Selection Option Type

The system must provide a `TerminalSelectionOption` type that:

- Defines a value for programmatic use
- Defines a label for display
- Trims whitespace from both values
- Validates both are not whitespace

**Acceptance Criteria:**

- [x] Constructor accepts value and label
- [x] Both properties are read-only
- [x] Constructor validates and trims both inputs

### FR-ABSTRACTION-014: Table Rendering Type

The system must provide a `TerminalTable` type that:

- Defines column headers
- Defines table rows
- Validates at least one column
- Validates column names are not whitespace
- Validates rows match column count

**Acceptance Criteria:**

- [x] Constructor accepts columns and rows
- [x] Both properties are read-only collections
- [x] Constructor validates all constraints

### FR-ABSTRACTION-015: Table Row Type

The system must provide a `TerminalTableRow` type that:

- Defines cell values
- Validates at least one cell
- Validates no cells are null

**Acceptance Criteria:**

- [x] Constructor accepts cells list
- [x] `Cells` property is read-only collection
- [x] Constructor validates all constraints

## Success Criteria

### SC-ABSTRACTION-001

All abstractions compile without warnings on .NET 11.

### SC-ABSTRACTION-002

All abstractions are compatible with Native AOT compilation.

### SC-ABSTRACTION-003

All abstractions can be unit tested without a terminal.

### SC-ABSTRACTION-004

Attribute validation happens at compile time or construction time.

### SC-ABSTRACTION-005

Factory methods on result types validate their inputs.

## Edge Cases

### EC-ABSTRACTION-001

Empty string or whitespace-only input to attribute constructors should throw `ArgumentException`.

### EC-ABSTRACTION-002

Null collections to result or prompt type constructors should throw `ArgumentNullException`.

### EC-ABSTRACTION-003

Negative position values for `[CliArgument]` should throw `ArgumentOutOfRangeException`.

### EC-ABSTRACTION-004

Empty options array for `[CliExample]` should throw `ArgumentException`.

### EC-ABSTRACTION-005

Rows with mismatched cell counts should throw `ArgumentException`.

## Non-Goals

- This spec does not cover the Spectre.Console implementation
- This spec does not cover the source generator implementation
- This spec does not cover command-line parsing logic (delegated to Spectre.Console.Cli)
- This spec does not cover internationalization

## Related Specifications

- [002: Spectre.Console Integration](./002-spectre-console-integration.md) — Implementation of `ITerminalSession`
- [003: Source Generator](./003-source-generator.md) — Code generation for commands
