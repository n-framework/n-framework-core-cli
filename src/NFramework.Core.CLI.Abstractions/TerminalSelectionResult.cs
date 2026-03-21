namespace NFramework.Core.CLI.Abstractions;

public sealed class TerminalSelectionResult
{
    private TerminalSelectionResult(string? selectedValue, bool wasCancelled)
    {
        SelectedValue = selectedValue;
        WasCancelled = wasCancelled;
    }

    public string? SelectedValue { get; }

    public bool WasCancelled { get; }

    public static TerminalSelectionResult Selected(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Selection value cannot be empty or whitespace.", nameof(value));

        return new TerminalSelectionResult(value.Trim(), false);
    }

    public static TerminalSelectionResult Cancelled()
    {
        return new TerminalSelectionResult(null, true);
    }
}
