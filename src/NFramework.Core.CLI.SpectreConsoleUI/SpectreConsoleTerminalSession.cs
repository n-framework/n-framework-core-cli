using NFramework.Core.CLI.Abstractions;
using Spectre.Console;

namespace NFramework.Core.CLI.SpectreConsoleUI;

public sealed class SpectreConsoleTerminalSession : ITerminalSession
{
    private readonly IAnsiConsole _console;
    private readonly TextWriter _errorWriter;
    private readonly ITextPromptRunner _textPromptRunner;
    private readonly ISelectionPromptRunner _selectionPromptRunner;

    public SpectreConsoleTerminalSession(IAnsiConsole console)
        : this(console, Console.Error, new SpectreTextPromptRunner(), new SpectreSelectionPromptRunner()) { }

    internal SpectreConsoleTerminalSession(
        IAnsiConsole console,
        TextWriter errorWriter,
        ITextPromptRunner textPromptRunner,
        ISelectionPromptRunner selectionPromptRunner
    )
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(errorWriter);
        ArgumentNullException.ThrowIfNull(textPromptRunner);
        ArgumentNullException.ThrowIfNull(selectionPromptRunner);

        _console = console;
        _errorWriter = errorWriter;
        _textPromptRunner = textPromptRunner;
        _selectionPromptRunner = selectionPromptRunner;
    }

    public bool IsInteractive => !Console.IsInputRedirected && !Console.IsOutputRedirected;

    public Task<TerminalTextInputResult> PromptForTextAsync(
        TerminalTextPrompt prompt,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_textPromptRunner.Prompt(_console, prompt));
    }

    public Task<TerminalSelectionResult> PromptForSelectionAsync(
        TerminalSelectionPrompt prompt,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_selectionPromptRunner.Prompt(_console, prompt));
    }

    public void WriteLine(string message)
    {
        _console.WriteLine(message);
    }

    public void WriteErrorLine(string message)
    {
        _errorWriter.WriteLine(message);
    }

    public void RenderTable(TerminalTable table)
    {
        ArgumentNullException.ThrowIfNull(table);

        Table renderedTable = new Table().Border(TableBorder.Rounded);
        foreach (string column in table.Columns)
            _ = renderedTable.AddColumn(column);

        foreach (TerminalTableRow row in table.Rows)
            _ = renderedTable.AddRow([.. row.Cells]);

        _console.Write(renderedTable);
    }
}

internal interface ITextPromptRunner
{
    TerminalTextInputResult Prompt(IAnsiConsole console, TerminalTextPrompt prompt);
}

internal interface ISelectionPromptRunner
{
    TerminalSelectionResult Prompt(IAnsiConsole console, TerminalSelectionPrompt prompt);
}

internal sealed class SpectreTextPromptRunner : ITextPromptRunner
{
    public TerminalTextInputResult Prompt(IAnsiConsole console, TerminalTextPrompt prompt)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(prompt);

        TextPrompt<string> renderedPrompt = new TextPrompt<string>(prompt.PromptText)
            .PromptStyle("green")
            .Validate(
                (string value) =>
                    string.IsNullOrWhiteSpace(value)
                        ? ValidationResult.Error(prompt.ValidationErrorMessage)
                        : ValidationResult.Success()
            );

        try
        {
            string submittedValue = console.Prompt(renderedPrompt);
            return TerminalTextInputResult.Submitted(submittedValue);
        }
        catch (OperationCanceledException)
        {
            return TerminalTextInputResult.Cancelled();
        }
        catch (InvalidOperationException)
        {
            return TerminalTextInputResult.Cancelled();
        }
    }
}

internal sealed class SpectreSelectionPromptRunner : ISelectionPromptRunner
{
    public TerminalSelectionResult Prompt(IAnsiConsole console, TerminalSelectionPrompt prompt)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(prompt);

        SelectionPrompt<TerminalSelectionOption> renderedPrompt = new SelectionPrompt<TerminalSelectionOption>()
            .Title(prompt.Title)
            .UseConverter(option => option.Label)
            .AddChoices(prompt.Options);

        try
        {
            TerminalSelectionOption selectedOption = console.Prompt(renderedPrompt);
            return TerminalSelectionResult.Selected(selectedOption.Value);
        }
        catch (OperationCanceledException)
        {
            return TerminalSelectionResult.Cancelled();
        }
        catch (InvalidOperationException)
        {
            return TerminalSelectionResult.Cancelled();
        }
    }
}
