# Specification: Spectre.Console Integration

**ID**: 002
**Title**: Spectre.Console Integration
**Status**: Implemented
**Created**: 2025-03-19
**Updated**: 2025-03-19

## Overview

This specification defines the Spectre.Console implementation of the CLI abstractions. It provides concrete implementations for `ITerminalSession` and dependency injection extensions for integrating with Microsoft.Extensions.DependencyInjection.

## User Scenarios & Testing

### Scenario 1: Display Output to Console

**Given** a command needs to write output
**When** the command calls `terminal.WriteLine()`
**Then** the text should be written to the console
**And** the output should support ANSI formatting if available

### Scenario 2: Display Error Output

**Given** a command needs to write an error
**When** the command calls `terminal.WriteErrorLine()`
**Then** the text should be written to stderr
**And** the output should not interfere with stdout

### Scenario 3: Prompt for Text Input

**Given** a command needs user input
**When** the terminal is interactive
**Then** `PromptForTextAsync()` should display a prompt
**And** accept user input
**And** validate that input is not empty
**And** return a submitted result

### Scenario 4: Handle Cancelled Text Input

**Given** a text prompt is displayed
**When** the user presses Ctrl+C
**Then** the method should return a cancelled result
**And** not throw an exception

### Scenario 5: Prompt for Selection

**Given** a command needs the user to choose from options
**When** the terminal is interactive
**Then** `PromptForSelectionAsync()` should display a selection menu
**And** highlight the selected option
**And** return a selected result with the value

### Scenario 6: Handle Cancelled Selection

**Given** a selection prompt is displayed
**When** the user presses Ctrl+C or Esc
**Then** the method should return a cancelled result
**And** not throw an exception

### Scenario 7: Detect Non-Interactive Mode

**Given** input or output is redirected
**When** the command checks `IsInteractive`
**Then** it should return false
**And** prompts should not be displayed

### Scenario 8: Render a Table

**Given** a command needs to display tabular data
**When** the command calls `RenderTable()`
**Then** the table should be formatted with borders
**And** columns should align properly
**And** rows should match column count

### Scenario 9: Register Services with DI

**Given** an application uses dependency injection
**When** the developer calls `AddCoreCliSpectreConsoleUi()`
**Then** `IAnsiConsole` should be registered as a singleton
**And** `ITerminalSession` should be registered as a singleton
**And** the console should use `AnsiConsole.Console`

## Requirements

### FR-SPECTRE-001: Terminal Session Implementation

The system must provide a `SpectreConsoleTerminalSession` class that:

- Implements `ITerminalSession`
- Accepts an `IAnsiConsole` in the constructor
- Delegates prompts to internal runner interfaces
- Writes errors to a separate TextWriter

**Acceptance Criteria:**

- [x] Class implements `ITerminalSession`
- [x] Constructor accepts `IAnsiConsole`
- [x] Internal constructor accepts console, error writer, and prompt runners
- [x] `IsInteractive` checks for redirected I/O

### FR-SPECTRE-002: Text Prompt Runner

The system must provide an `ITextPromptRunner` interface and implementation that:

- Creates a Spectre.Console `TextPrompt<string>`
- Configures prompt style as green
- Validates input is not empty
- Returns submitted value or cancellation

**Acceptance Criteria:**

- [x] `ITextPromptRunner` interface exists
- [x] `SpectreTextPromptRunner` implements the interface
- [x] Prompt uses validation error message from `TerminalTextPrompt`
- [x] Returns `TerminalTextInputResult.Submitted()` on success
- [x] Returns `TerminalTextInputResult.Cancelled()` on `OperationCanceledException`
- [x] Returns `TerminalTextInputResult.Cancelled()` on `InvalidOperationException`

### FR-SPECTRE-003: Selection Prompt Runner

The system must provide an `ISelectionPromptRunner` interface and implementation that:

- Creates a Spectre.Console `SelectionPrompt<TerminalSelectionOption>`
- Configures title from `TerminalSelectionPrompt`
- Adds all options to the prompt
- Uses label converter for display
- Returns selected value or cancellation

**Acceptance Criteria:**

- [x] `ISelectionPromptRunner` interface exists
- [x] `SpectreSelectionPromptRunner` implements the interface
- [x] Prompt title is set from `TerminalSelectionPrompt.Title`
- [x] All options are added to the prompt
- [x] Label converter displays option labels
- [x] Returns `TerminalSelectionResult.Selected()` on success
- [x] Returns `TerminalSelectionResult.Cancelled()` on `OperationCanceledException`
- [x] Returns `TerminalSelectionResult.Cancelled()` on `InvalidOperationException`

### FR-SPECTRE-004: Table Rendering

The `SpectreConsoleTerminalSession.RenderTable()` method must:

- Create a Spectre.Console `Table` with rounded borders
- Add all columns from `TerminalTable`
- Add all rows from `TerminalTable`
- Write the table to the console

**Acceptance Criteria:**

- [x] Table border is set to `TableBorder.Rounded`
- [x] All columns are added in order
- [x] All rows are added with cells expanded
- [x] Table is written to `_console`

### FR-SPECTRE-005: Dependency Injection Extension

The system must provide a `AddCoreCliSpectreConsoleUi()` extension method that:

- Registers `IAnsiConsole` as a singleton using `AnsiConsole.Console`
- Registers `ITerminalSession` as a singleton mapping to `SpectreConsoleTerminalSession`
- Returns the service collection for chaining

**Acceptance Criteria:**

- [x] Extension method exists on `IServiceCollection`
- [x] `IAnsiConsole` is registered with factory returning `AnsiConsole.Console`
- [x] `ITerminalSession` is registered as singleton
- [x] Method returns `IServiceCollection` for chaining

### FR-SPECTRE-006: Interactive Detection

The `IsInteractive` property must:

- Return false if input is redirected
- Return false if output is redirected
- Return true otherwise

**Acceptance Criteria:**

- [x] Checks `Console.IsInputRedirected`
- [x] Checks `Console.IsOutputRedirected`
- [x] Returns true only if both are false

## Success Criteria

### SC-SPECTRE-001

All methods handle null arguments with `ArgumentNullException`.

### SC-SPECTRE-002

Prompt runners handle cancellation gracefully without throwing.

### SC-SPECTRE-003

Table rendering validates the table argument.

### SC-SPECTRE-004

DI extension method properly registers all services.

### SC-SPECTRE-005

Internal constructors are accessible to unit tests.

## Edge Cases

### EC-SPECTRE-001

Null console passed to constructor should throw `ArgumentNullException`.

### EC-SPECTRE-002

Null error writer passed to internal constructor should throw `ArgumentNullException`.

### EC-SPECTRE-003

Null prompt runners passed to internal constructor should throw `ArgumentNullException`.

### EC-SPECTRE-004

Null table passed to `RenderTable()` should throw `ArgumentNullException`.

### EC-SPECTRE-005

Null console or prompt passed to prompt runners should throw `ArgumentNullException`.

## Non-Goals

- This spec does not cover customizing prompt styles or colors
- This spec does not cover advanced Spectre.Console features (progress bars, spinners)
- This spec does not cover ANSI escape sequence handling beyond Spectre.Console
- This spec does not cover alternative console implementations

## Related Specifications

- [001: CLI Abstractions](./001-cli-abstractions.md) — Interface definitions
- [003: Source Generator](./003-source-generator.md) — Command generation

## Implementation Notes

### Internal Interfaces

`ITextPromptRunner` and `ISelectionPromptRunner` are internal to enable unit testing without directly depending on Spectre.Console prompts.

### Factory Pattern

The DI extension uses a factory lambda for `IAnsiConsole` to ensure the same instance is used throughout the application lifetime.

### Error Output

Error output is written to a separate `TextWriter` (defaulting to `Console.Error`) to properly separate stdout and stderr.

### Cancellation Handling

Both prompt runners catch `OperationCanceledException` and `InvalidOperationException` (thrown by Spectre.Console when Ctrl+C is pressed) and return cancellation results instead of propagating exceptions.
