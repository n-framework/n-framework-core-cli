namespace NFramework.Core.CLI.Abstractions;

public sealed class TerminalSelectionOption
{
    public TerminalSelectionOption(string value, string label)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Selection value cannot be empty or whitespace.", nameof(value));

        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Selection label cannot be empty or whitespace.", nameof(label));

        Value = value.Trim();
        Label = label.Trim();
    }

    public string Value { get; }

    public string Label { get; }
}
