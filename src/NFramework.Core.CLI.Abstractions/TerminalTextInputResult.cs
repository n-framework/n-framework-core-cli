namespace NFramework.Core.CLI.Abstractions;

public sealed class TerminalTextInputResult
{
    private TerminalTextInputResult(string? value, bool wasCancelled)
    {
        Value = value;
        WasCancelled = wasCancelled;
    }

    public string? Value { get; }

    public bool WasCancelled { get; }

    public static TerminalTextInputResult Submitted(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Terminal input cannot be empty or whitespace.", nameof(value));

        return new TerminalTextInputResult(value.Trim(), false);
    }

    public static TerminalTextInputResult Cancelled()
    {
        return new TerminalTextInputResult(null, true);
    }
}
