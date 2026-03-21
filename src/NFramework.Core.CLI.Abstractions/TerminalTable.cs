namespace NFramework.Core.CLI.Abstractions;

public sealed class TerminalTable
{
    public TerminalTable(IReadOnlyList<string> columns, IReadOnlyList<TerminalTableRow> rows)
    {
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(rows);

        if (columns.Count == 0)
            throw new ArgumentException("Table must define at least one column.", nameof(columns));

        if (columns.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Table columns cannot be empty or whitespace.", nameof(columns));

        if (rows.Any(row => row.Cells.Count != columns.Count))
            throw new ArgumentException("Each table row must match the column count.", nameof(rows));

        Columns = columns;
        Rows = rows;
    }

    public IReadOnlyList<string> Columns { get; }

    public IReadOnlyList<TerminalTableRow> Rows { get; }
}
