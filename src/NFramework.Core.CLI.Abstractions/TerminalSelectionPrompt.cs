namespace NFramework.Core.CLI.Abstractions;

public sealed class TerminalSelectionPrompt
{
    public TerminalSelectionPrompt(string title, IReadOnlyList<TerminalSelectionOption> options)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Prompt title cannot be empty or whitespace.", nameof(title));

        ArgumentNullException.ThrowIfNull(options);

        if (options.Count == 0)
            throw new ArgumentException("Selection prompt must include at least one option.", nameof(options));

        Title = title;
        Options = options;
    }

    public string Title { get; }

    public IReadOnlyList<TerminalSelectionOption> Options { get; }
}
