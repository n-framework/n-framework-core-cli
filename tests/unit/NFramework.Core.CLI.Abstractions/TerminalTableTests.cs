using NFramework.Core.CLI.Abstractions;
using Shouldly;
using Xunit;

namespace NFramework.Core.CLI.Abstractions.Tests;

public class TerminalTableTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesTable_WithValidColumnsAndEmptyRows()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "Name", "Age", "City" };
            IReadOnlyList<TerminalTableRow> rows = Array.Empty<TerminalTableRow>();

            // Act
            TerminalTable table = new TerminalTable(columns, rows);

            // Assert
            table.Columns.Count.ShouldBe(3);
            table.Columns[0].ShouldBe("Name");
            table.Columns[1].ShouldBe("Age");
            table.Columns[2].ShouldBe("City");
            Assert.Empty(table.Rows);
        }

        [Fact]
        public void CreatesTable_WithValidColumnsAndRows()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "Name", "Age" };
            IReadOnlyList<TerminalTableRow> rows = new[]
            {
                new TerminalTableRow(new[] { "Alice", "30" }),
                new TerminalTableRow(new[] { "Bob", "25" }),
            };

            // Act
            TerminalTable table = new TerminalTable(columns, rows);

            // Assert
            table.Columns.Count.ShouldBe(2);
            table.Rows.Count.ShouldBe(2);
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenColumnsIsNull()
        {
            // Arrange
            IReadOnlyList<TerminalTableRow> rows = Array.Empty<TerminalTableRow>();

            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() => new TerminalTable(null!, rows));
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenRowsIsNull()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "Name" };

            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() => new TerminalTable(columns, null!));
        }

        [Fact]
        public void ThrowsArgumentException_WhenColumnsAreEmpty()
        {
            // Arrange
            IReadOnlyList<string> columns = Array.Empty<string>();
            IReadOnlyList<TerminalTableRow> rows = Array.Empty<TerminalTableRow>();

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalTable(columns, rows));
        }

        [Fact]
        public void ThrowsArgumentException_WhenColumnsContainEmptyString()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "Name", "", "City" };
            IReadOnlyList<TerminalTableRow> rows = Array.Empty<TerminalTableRow>();

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalTable(columns, rows));
        }

        [Fact]
        public void ThrowsArgumentException_WhenColumnsContainWhitespace()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "Name", "   ", "City" };
            IReadOnlyList<TerminalTableRow> rows = Array.Empty<TerminalTableRow>();

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalTable(columns, rows));
        }

        [Fact]
        public void ThrowsArgumentException_WhenFirstColumnIsEmpty()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "", "Age", "City" };
            IReadOnlyList<TerminalTableRow> rows = Array.Empty<TerminalTableRow>();

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalTable(columns, rows));
        }

        [Fact]
        public void ThrowsArgumentException_WhenRowsHaveMismatchedCellCount()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "Name", "Age" };
            IReadOnlyList<TerminalTableRow> rows = new[]
            {
                new TerminalTableRow(new[] { "Alice", "30" }),
                new TerminalTableRow(new[] { "Bob" }), // Mismatched
            };

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalTable(columns, rows));
        }
    }

    public class Properties
    {
        [Fact]
        public void ColumnsProperty_IsReadOnly()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "Name" };
            IReadOnlyList<TerminalTableRow> rows = Array.Empty<TerminalTableRow>();
            TerminalTable table = new TerminalTable(columns, rows);

            // Act & Assert
            IReadOnlyList<string> cols = table.Columns;
            _ = cols; // Suppress IDE0058
            _ = cols.ShouldNotBeNull();
            _ = cols.ShouldHaveSingleItem();
        }

        [Fact]
        public void RowsProperty_IsReadOnly()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "Name" };
            IReadOnlyList<TerminalTableRow> rows = new[] { new TerminalTableRow(new[] { "Alice" }) };
            TerminalTable table = new TerminalTable(columns, rows);

            // Act & Assert
            IReadOnlyList<TerminalTableRow> rowList = table.Rows;
            _ = rowList;
            _ = rowList.ShouldNotBeNull();
            _ = rowList.ShouldHaveSingleItem();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesSingleColumnTable()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "ID" };
            IReadOnlyList<TerminalTableRow> rows = new[]
            {
                new TerminalTableRow(new[] { "1" }),
                new TerminalTableRow(new[] { "2" }),
            };

            // Act
            TerminalTable table = new TerminalTable(columns, rows);
            _ = table; // Suppress IDE0058

            // Assert
            _ = table.Columns.ShouldHaveSingleItem();
            table.Rows.Count.ShouldBe(2);
        }

        [Fact]
        public void HandlesLargeNumberOfColumns()
        {
            // Arrange
            IReadOnlyList<string> columns = new[]
            {
                "Col1",
                "Col2",
                "Col3",
                "Col4",
                "Col5",
                "Col6",
                "Col7",
                "Col8",
                "Col9",
                "Col10",
            };
            IReadOnlyList<TerminalTableRow> rows = new[]
            {
                new TerminalTableRow(Enumerable.Range(1, 10).Select(i => $"Value{i}").ToArray()),
            };

            // Act
            TerminalTable table = new TerminalTable(columns, rows);

            // Assert
            table.Columns.Count.ShouldBe(10);
            table.Rows[0].Cells.Count.ShouldBe(10);
        }

        [Fact]
        public void HandlesSpecialCharactersInColumnNames()
        {
            // Arrange
            IReadOnlyList<string> columns = new[] { "User ID", "Email-Address", "File.Path" };
            IReadOnlyList<TerminalTableRow> rows = Array.Empty<TerminalTableRow>();

            // Act
            TerminalTable table = new TerminalTable(columns, rows);

            // Assert
            table.Columns.Count.ShouldBe(3);
            table.Columns[0].ShouldBe("User ID");
            table.Columns[1].ShouldBe("Email-Address");
            table.Columns[2].ShouldBe("File.Path");
        }
    }
}

public class TerminalTableRowTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesRow_WithValidCells()
        {
            // Arrange
            IReadOnlyList<string> cells = new[] { "Alice", "30", "New York" };

            // Act
            TerminalTableRow row = new TerminalTableRow(cells);

            // Assert
            row.Cells.Count.ShouldBe(3);
            row.Cells[0].ShouldBe("Alice");
            row.Cells[1].ShouldBe("30");
            row.Cells[2].ShouldBe("New York");
        }

        [Fact]
        public void CreatesRow_WithSingleCell()
        {
            // Arrange
            IReadOnlyList<string> cells = new[] { "Value" };

            // Act
            TerminalTableRow row = new TerminalTableRow(cells);

            // Assert
            _ = row.Cells.ShouldHaveSingleItem();
            row.Cells[0].ShouldBe("Value");
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenCellsIsNull()
        {
            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() => new TerminalTableRow(null!));
        }

        [Fact]
        public void ThrowsArgumentException_WhenCellsAreEmpty()
        {
            // Arrange
            IReadOnlyList<string> cells = Array.Empty<string>();

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalTableRow(cells));
        }

        [Fact]
        public void ThrowsArgumentException_WhenCellsContainNull()
        {
            // Arrange
            IReadOnlyList<string> cells = new[] { "Value1", null!, "Value3" };

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalTableRow(cells));
        }
    }

    public class Properties
    {
        [Fact]
        public void CellsProperty_IsReadOnly()
        {
            // Arrange
            IReadOnlyList<string> cells = new[] { "Test" };
            TerminalTableRow row = new TerminalTableRow(cells);

            // Act & Assert
            IReadOnlyList<string> rowCells = row.Cells;
            _ = rowCells; // Suppress IDE0058
            _ = rowCells.ShouldNotBeNull();
            _ = rowCells.ShouldHaveSingleItem();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesEmptyStringCells()
        {
            // Arrange
            IReadOnlyList<string> cells = new[] { "", "", "" };

            // Act
            TerminalTableRow row = new TerminalTableRow(cells);

            // Assert
            row.Cells.Count.ShouldBe(3);
            Assert.All(row.Cells, cell => Assert.Empty(cell));
        }

        [Fact]
        public void HandlesWhitespaceStringCells()
        {
            // Arrange
            IReadOnlyList<string> cells = new[] { "  ", "\t", "\n" };

            // Act
            TerminalTableRow row = new TerminalTableRow(cells);

            // Assert
            row.Cells.Count.ShouldBe(3);
        }

        [Fact]
        public void HandlesSpecialCharactersInCells()
        {
            // Arrange
            IReadOnlyList<string> cells = new[] { "Hello, World!", "test@example.com", "C:\\Path\\To\\File.txt" };

            // Act
            TerminalTableRow row = new TerminalTableRow(cells);

            // Assert
            row.Cells.Count.ShouldBe(3);
            row.Cells[0].ShouldBe("Hello, World!");
            row.Cells[1].ShouldBe("test@example.com");
            row.Cells[2].ShouldBe("C:\\Path\\To\\File.txt");
        }
    }
}
