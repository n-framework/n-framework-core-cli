namespace NFramework.Core.CLI.Abstractions;

public interface ITerminalSession
{
    bool IsInteractive { get; }

    Task<TerminalTextInputResult> PromptForTextAsync(TerminalTextPrompt prompt, CancellationToken cancellationToken);

    Task<TerminalSelectionResult> PromptForSelectionAsync(
        TerminalSelectionPrompt prompt,
        CancellationToken cancellationToken
    );

    void WriteLine(string message);

    void WriteErrorLine(string message);

    void RenderTable(TerminalTable table);
}
