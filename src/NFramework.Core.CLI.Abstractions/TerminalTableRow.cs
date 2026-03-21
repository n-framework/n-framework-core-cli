namespace NFramework.Core.CLI.Abstractions;

public sealed class TerminalTableRow
{
    public TerminalTableRow(IReadOnlyList<string> cells)
    {
        ArgumentNullException.ThrowIfNull(cells);

        if (cells.Count == 0)
            throw new ArgumentException("Table row must define at least one cell.", nameof(cells));

        if (cells.Any(cell => cell is null))
            throw new ArgumentException("Table row cells cannot be null.", nameof(cells));

        Cells = cells;
    }

    public IReadOnlyList<string> Cells { get; }
}
