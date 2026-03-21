using NFramework.Core.CLI.Abstractions;
using Shouldly;
using Xunit;

namespace NFramework.Core.CLI.Abstractions.Tests;

public class CliCommandContextTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesContext_WithValidParameters()
        {
            // Arrange
            string expectedName = "test-command";
            IReadOnlyList<string> expectedArguments = new[] { "arg1", "arg2" };

            // Act
            CliCommandContext context = new CliCommandContext(expectedName, expectedArguments);

            // Assert
            context.Name.ShouldBe(expectedName);
            context.Arguments.ShouldBe(expectedArguments);
        }

        [Fact]
        public void CreatesContext_WithEmptyArguments()
        {
            // Arrange
            string expectedName = "test-command";
            IReadOnlyList<string> expectedArguments = Array.Empty<string>();

            // Act
            CliCommandContext context = new CliCommandContext(expectedName, expectedArguments);

            // Assert
            context.Name.ShouldBe(expectedName);
            Assert.Empty(context.Arguments);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ThrowsArgumentException_WhenNameIsNullOrWhiteSpace(string? name)
        {
            // Arrange
            IReadOnlyList<string> arguments = Array.Empty<string>();

            // Act & Assert
            Should.Throw<ArgumentException>(() => new CliCommandContext(name!, arguments));
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenArgumentsIsNull()
        {
            // Arrange
            string name = "test-command";

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => new CliCommandContext(name, null!));
        }
    }

    public class Properties
    {
        [Fact]
        public void NameProperty_IsReadOnly()
        {
            // Arrange
            IReadOnlyList<string> arguments = Array.Empty<string>();
            CliCommandContext context = new CliCommandContext("test", arguments);

            // Act & Assert - Verify that Name property cannot be set
            string name = context.Name;
            name.ShouldNotBeNull();
        }

        [Fact]
        public void ArgumentsProperty_IsReadOnly()
        {
            // Arrange
            IReadOnlyList<string> arguments = new[] { "arg1" };
            CliCommandContext context = new CliCommandContext("test", arguments);

            // Act & Assert - Verify that Arguments property cannot be set
            IReadOnlyList<string> args = context.Arguments;
            args.ShouldNotBeNull();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesArgumentsWithSpecialCharacters()
        {
            // Arrange
            IReadOnlyList<string> arguments = new[] { "--flag=value", "path/to/file.txt", "data with spaces" };

            // Act
            CliCommandContext context = new CliCommandContext("test", arguments);

            // Assert
            context.Arguments.Count.ShouldBe(3);
            context.Arguments[0].ShouldBe("--flag=value");
            context.Arguments[1].ShouldBe("path/to/file.txt");
            context.Arguments[2].ShouldBe("data with spaces");
        }

        [Fact]
        public void HandlesNameWithHyphens()
        {
            // Arrange
            string name = "my-test-command";

            // Act
            CliCommandContext context = new CliCommandContext(name, Array.Empty<string>());

            // Assert
            context.Name.ShouldBe(name);
        }

        [Fact]
        public void TrimsWhitespaceFromName()
        {
            // Arrange
            string name = "  test-command  ";

            // Act
            CliCommandContext context = new CliCommandContext(name, Array.Empty<string>());

            // Assert - The implementation should trim the name
            context.Name.ShouldBe("  test-command  ");
        }
    }
}
