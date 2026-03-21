using NFramework.Core.CLI.Abstractions;
using Shouldly;
using Xunit;

namespace NFramework.Core.CLI.Abstractions.Tests;

public class TerminalTextInputResultTests
{
    public class SubmittedMethod
    {
        [Fact]
        public void ReturnsResultWithValue()
        {
            // Act
            TerminalTextInputResult result = TerminalTextInputResult.Submitted("test value");

            // Assert
            result.Value.ShouldBe("test value");
            result.WasCancelled.ShouldBeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void ThrowsArgumentException_WhenValueIsNullOrWhiteSpace(string? value)
        {
            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => TerminalTextInputResult.Submitted(value!));
        }

        [Fact]
        public void TrimsWhitespace_FromValue()
        {
            // Act
            TerminalTextInputResult result = TerminalTextInputResult.Submitted("  test value  ");

            // Assert
            result.Value.ShouldBe("test value");
        }
    }

    public class CancelledMethod
    {
        [Fact]
        public void ReturnsCancelledResult()
        {
            // Act
            TerminalTextInputResult result = TerminalTextInputResult.Cancelled();

            // Assert
            result.Value.ShouldBeNull();
            result.WasCancelled.ShouldBeTrue();
        }
    }
}
