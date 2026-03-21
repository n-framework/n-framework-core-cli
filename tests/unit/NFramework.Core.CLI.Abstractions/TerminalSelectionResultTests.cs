using NFramework.Core.CLI.Abstractions;
using Shouldly;
using Xunit;

namespace NFramework.Core.CLI.Abstractions.Tests;

public class TerminalSelectionResultTests
{
    public class SelectedMethod
    {
        [Fact]
        public void ReturnsResultWithValue()
        {
            // Act
            TerminalSelectionResult result = TerminalSelectionResult.Selected("option1");

            // Assert
            result.SelectedValue.ShouldBe("option1");
            result.WasCancelled.ShouldBeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void ThrowsArgumentException_WhenValueIsNullOrWhiteSpace(string? value)
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => TerminalSelectionResult.Selected(value!));
        }

        [Fact]
        public void TrimsWhitespace_FromValue()
        {
            // Act
            TerminalSelectionResult result = TerminalSelectionResult.Selected("  option1  ");

            // Assert
            result.SelectedValue.ShouldBe("option1");
        }
    }

    public class CancelledMethod
    {
        [Fact]
        public void ReturnsCancelledResult()
        {
            // Act
            TerminalSelectionResult result = TerminalSelectionResult.Cancelled();

            // Assert
            result.SelectedValue.ShouldBeNull();
            result.WasCancelled.ShouldBeTrue();
        }
    }
}
