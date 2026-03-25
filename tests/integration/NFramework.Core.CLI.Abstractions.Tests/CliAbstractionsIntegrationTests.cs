using NFramework.Core.CLI.Abstractions;
using Shouldly;
using Xunit;

namespace NFramework.Core.CLI.Abstractions.Tests;

public class CliAbstractionsIntegrationTests
{
    [Fact]
    public void TerminalTextInputResult_Submitted_WithValidInput_ReturnsSuccessResult()
    {
        // Arrange
        string validInput = "test input";

        // Act
        TerminalTextInputResult result = TerminalTextInputResult.Submitted(validInput);

        // Assert
        result.WasCancelled.ShouldBeFalse();
        result.Value.ShouldBe(validInput);
        result.WasCancelled.ShouldBeFalse();
    }

    [Fact]
    public void TerminalTextInputResult_Submitted_WithNullInput_ThrowsArgumentException()
    {
        // Act & Assert
        _ = Should.Throw<ArgumentException>(() => TerminalTextInputResult.Submitted(null!));
    }

    [Fact]
    public void TerminalTextInputResult_Submitted_WithEmptyInput_ThrowsArgumentException()
    {
        // Act & Assert
        _ = Should.Throw<ArgumentException>(() => TerminalTextInputResult.Submitted(""));
    }

    [Fact]
    public void TerminalTextInputResult_Submitted_WithWhitespaceInput_ThrowsArgumentException()
    {
        // Act & Assert
        _ = Should.Throw<ArgumentException>(() => TerminalTextInputResult.Submitted("   "));
    }

    [Fact]
    public void TerminalTextInputResult_Submitted_WithWhitespace_TrimsInput()
    {
        // Arrange
        string input = "  test input  ";

        // Act
        TerminalTextInputResult result = TerminalTextInputResult.Submitted(input);

        // Assert
        result.Value.ShouldBe("test input");
    }

    [Fact]
    public void TerminalTextInputResult_Cancelled_ReturnsCancelledResult()
    {
        // Act
        TerminalTextInputResult result = TerminalTextInputResult.Cancelled();

        // Assert
        result.WasCancelled.ShouldBeTrue();
        result.Value.ShouldBeNull();
        result.WasCancelled.ShouldBeTrue();
    }

    [Fact]
    public void TerminalSelectionResult_Selected_WithValidValue_ReturnsSelectedResult()
    {
        // Arrange
        string selectedValue = "option1";

        // Act
        TerminalSelectionResult result = TerminalSelectionResult.Selected(selectedValue);

        // Assert
        result.WasCancelled.ShouldBeFalse();
        result.SelectedValue.ShouldBe(selectedValue);
        result.WasCancelled.ShouldBeFalse();
    }

    [Fact]
    public void TerminalSelectionResult_Selected_WithNullValue_ThrowsArgumentException()
    {
        // Act & Assert
        _ = Should.Throw<ArgumentException>(() => TerminalSelectionResult.Selected(null!));
    }

    [Fact]
    public void TerminalSelectionResult_Selected_WithEmptyValue_ThrowsArgumentException()
    {
        // Act & Assert
        _ = Should.Throw<ArgumentException>(() => TerminalSelectionResult.Selected(""));
    }

    [Fact]
    public void TerminalSelectionResult_Selected_WithWhitespaceValue_ThrowsArgumentException()
    {
        // Act & Assert
        _ = Should.Throw<ArgumentException>(() => TerminalSelectionResult.Selected("   "));
    }

    [Fact]
    public void TerminalSelectionResult_Selected_WithWhitespace_TrimsValue()
    {
        // Arrange
        string selectedValue = "  option1  ";

        // Act
        TerminalSelectionResult result = TerminalSelectionResult.Selected(selectedValue);

        // Assert
        result.SelectedValue.ShouldBe("option1");
    }

    [Fact]
    public void TerminalSelectionResult_Cancelled_ReturnsCancelledResult()
    {
        // Act
        TerminalSelectionResult result = TerminalSelectionResult.Cancelled();

        // Assert
        result.WasCancelled.ShouldBeTrue();
        result.SelectedValue.ShouldBeNull();
        result.WasCancelled.ShouldBeTrue();
    }

    [Fact]
    public void TerminalTextPrompt_WithValidProperties_ConstructsCorrectly()
    {
        // Arrange
        TerminalTextPrompt prompt = new TerminalTextPrompt("Enter your name:", "Value cannot be empty");

        // Act & Assert
        prompt.PromptText.ShouldBe("Enter your name:");
        prompt.ValidationErrorMessage.ShouldBe("Value cannot be empty");
    }

    [Fact]
    public void TerminalTextPrompt_WithCustomValidationMessage_ConstructsCorrectly()
    {
        // Arrange
        TerminalTextPrompt prompt = new TerminalTextPrompt("Enter value:", "Value must not be empty") { };

        // Act & Assert
        prompt.PromptText.ShouldBe("Enter value:");
        prompt.ValidationErrorMessage.ShouldBe("Value must not be empty");
    }

    [Fact]
    public void TerminalSelectionPrompt_WithValidOptions_ConstructsCorrectly()
    {
        // Arrange
        TerminalSelectionOption[] options =
        [
            new TerminalSelectionOption("1", "Option 1"),
            new TerminalSelectionOption("2", "Option 2"),
        ];
        TerminalSelectionPrompt prompt = new TerminalSelectionPrompt("Choose an option:", options);

        // Act & Assert
        prompt.Title.ShouldBe("Choose an option:");
        prompt.Options.Count.ShouldBe(2);
        prompt.Options[0].Value.ShouldBe("1");
        prompt.Options[0].Label.ShouldBe("Option 1");
        prompt.Options[1].Value.ShouldBe("2");
        prompt.Options[1].Label.ShouldBe("Option 2");
    }

    [Fact]
    public void TerminalSelectionPrompt_WithEmptyOptions_ThrowsArgumentException()
    {
        // Arrange
        TerminalSelectionOption[] options = [];

        // Act & Assert
        _ = Should.Throw<ArgumentException>(() => new TerminalSelectionPrompt("Choose an option:", options));
    }

    [Fact]
    public void TerminalTable_ConstructsWithColumns()
    {
        // Arrange
        string[] columns = ["Name", "Age", "City"];
        IReadOnlyList<TerminalTableRow> rows = Array.Empty<TerminalTableRow>();

        // Act
        TerminalTable table = new TerminalTable(columns, rows);

        // Assert
        table.Columns.Count.ShouldBe(3);
        table.Columns[0].ShouldBe("Name");
        table.Columns[1].ShouldBe("Age");
        table.Columns[2].ShouldBe("City");
        table.Rows.Count.ShouldBe(0);
    }

    [Fact]
    public void TerminalTable_AddRow_AddsRowCorrectly()
    {
        // Arrange
        string[] columns = ["Name", "Age"];
        IReadOnlyList<TerminalTableRow> rows = new[]
        {
            new TerminalTableRow(new[] { "Alice", "30" }),
            new TerminalTableRow(new[] { "Bob", "25" }),
        };

        // Act
        TerminalTable table = new TerminalTable(columns, rows);

        // Assert
        table.Rows.Count.ShouldBe(2);
        table.Rows[0].Cells[0].ShouldBe("Alice");
        table.Rows[0].Cells[1].ShouldBe("30");
        table.Rows[1].Cells[0].ShouldBe("Bob");
        table.Rows[1].Cells[1].ShouldBe("25");
    }

    [Fact]
    public void TerminalTable_AddRow_WithCorrectCellCount()
    {
        // Arrange
        string[] columns = ["Name", "Age"];
        IReadOnlyList<TerminalTableRow> rows = new[] { new TerminalTableRow(new[] { "Alice", "30" }) };

        // Act
        TerminalTable table = new TerminalTable(columns, rows);

        // Assert
        table.Rows.Count.ShouldBe(1);
        table.Rows[0].Cells.Count.ShouldBe(2);
    }

    [Fact]
    public void TerminalTableRow_ConstructsWithCells()
    {
        // Arrange
        string[] cells = ["Cell1", "Cell2", "Cell3"];

        // Act
        TerminalTableRow row = new TerminalTableRow(cells);

        // Assert
        row.Cells.Count.ShouldBe(3);
        row.Cells[0].ShouldBe("Cell1");
        row.Cells[1].ShouldBe("Cell2");
        row.Cells[2].ShouldBe("Cell3");
    }

    [Fact]
    public void TerminalSelectionOption_ConstructsWithValueAndLabel()
    {
        // Arrange & Act
        TerminalSelectionOption option = new TerminalSelectionOption("value1", "Option 1");

        // Assert
        option.Value.ShouldBe("value1");
        option.Label.ShouldBe("Option 1");
    }
}
