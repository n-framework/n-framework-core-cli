using NFramework.Core.CLI.Abstractions;
using Shouldly;
using Xunit;

namespace NFramework.Core.CLI.Abstractions.Tests;

public class CliApplicationAttributeTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesAttribute_WithValidName()
        {
            // Arrange
            string expectedName = "myapp";

            // Act
            CliApplicationAttribute attribute = new CliApplicationAttribute(expectedName);

            // Assert
            attribute.Name.ShouldBe(expectedName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenNameIsNullOrWhiteSpace(string? name)
        {
            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new CliApplicationAttribute(name!));
        }
    }

    public class Properties
    {
        [Fact]
        public void NameProperty_IsReadOnly()
        {
            // Arrange
            CliApplicationAttribute attribute = new CliApplicationAttribute("test");

            // Act & Assert
            string name = attribute.Name;
            _ = name.ShouldNotBeNull();
        }

        [Fact]
        public void ValidateExamplesProperty_HasDefaultValue()
        {
            // Arrange
            CliApplicationAttribute attribute = new CliApplicationAttribute("test");

            // Act & Assert
            attribute.ValidateExamples.ShouldBeTrue();
        }

        [Fact]
        public void ValidateExamplesProperty_CanBeSet()
        {
            // Arrange & Act
            CliApplicationAttribute attribute = new CliApplicationAttribute("test") { ValidateExamples = false };

            // Assert
            attribute.ValidateExamples.ShouldBeFalse();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesNameWithHyphens()
        {
            // Arrange
            string name = "my-test-app";

            // Act
            CliApplicationAttribute attribute = new CliApplicationAttribute(name);

            // Assert
            attribute.Name.ShouldBe(name);
        }

        [Fact]
        public void HandlesNameWithUnderscores()
        {
            // Arrange
            string name = "my_test_app";

            // Act
            CliApplicationAttribute attribute = new CliApplicationAttribute(name);

            // Assert
            attribute.Name.ShouldBe(name);
        }

        [Fact]
        public void HandlesNameWithNumbers()
        {
            // Arrange
            string name = "app123";

            // Act
            CliApplicationAttribute attribute = new CliApplicationAttribute(name);

            // Assert
            attribute.Name.ShouldBe(name);
        }
    }
}

public class CliCommandAttributeTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesAttribute_WithValidNameAndDescription()
        {
            // Arrange
            string expectedName = "greet";
            string expectedDescription = "Prints a greeting";

            // Act
            CliCommandAttribute attribute = new CliCommandAttribute(expectedName, expectedDescription);

            // Assert
            attribute.Name.ShouldBe(expectedName);
            attribute.Description.ShouldBe(expectedDescription);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenNameIsNullOrWhiteSpace(string? name)
        {
            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new CliCommandAttribute(name!, "description"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenDescriptionIsNullOrWhiteSpace(string? description)
        {
            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new CliCommandAttribute("test", description!));
        }
    }

    public class Properties
    {
        [Fact]
        public void NameProperty_IsReadOnly()
        {
            // Arrange
            CliCommandAttribute attribute = new CliCommandAttribute("test", "description");

            // Act & Assert
            string name = attribute.Name;
            _ = name.ShouldNotBeNull();
        }

        [Fact]
        public void DescriptionProperty_IsReadOnly()
        {
            // Arrange
            CliCommandAttribute attribute = new CliCommandAttribute("test", "description");

            // Act & Assert
            string description = attribute.Description;
            _ = description.ShouldNotBeNull();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesNameWithHyphens()
        {
            // Arrange
            string name = "test-command";

            // Act
            CliCommandAttribute attribute = new CliCommandAttribute(name, "description");

            // Assert
            attribute.Name.ShouldBe(name);
        }

        [Fact]
        public void HandlesNameWithColons()
        {
            // Arrange
            string name = "db:migrate";

            // Act
            CliCommandAttribute attribute = new CliCommandAttribute(name, "description");

            // Assert
            attribute.Name.ShouldBe(name);
        }

        [Fact]
        public void HandlesLongDescription()
        {
            // Arrange
            string description = new string('a', 1000);

            // Act
            CliCommandAttribute attribute = new CliCommandAttribute("test", description);

            // Assert
            attribute.Description.Length.ShouldBe(1000);
        }
    }
}

public class CliArgumentAttributeTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesAttribute_WithValidParameters()
        {
            // Arrange
            int expectedPosition = 0;
            string expectedValueName = "input";

            // Act
            CliArgumentAttribute attribute = new CliArgumentAttribute(expectedPosition, expectedValueName);

            // Assert
            attribute.Position.ShouldBe(expectedPosition);
            attribute.ValueName.ShouldBe(expectedValueName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenValueNameIsNullOrWhiteSpace(string? valueName)
        {
            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new CliArgumentAttribute(0, valueName!));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void ThrowsArgumentOutOfRangeException_WhenPositionIsNegative(int position)
        {
            // Arrange
            string name = "test";

            // Act & Assert
            _ = Should.Throw<ArgumentOutOfRangeException>(() => new CliArgumentAttribute(position, name));
        }
    }

    public class Properties
    {
        [Fact]
        public void PositionProperty_IsReadOnly()
        {
            // Arrange
            CliArgumentAttribute attribute = new CliArgumentAttribute(0, "test");

            // Act & Assert
            int position = attribute.Position;
            position.ShouldBe(0);
        }

        [Fact]
        public void ValueNameProperty_IsReadOnly()
        {
            // Arrange
            CliArgumentAttribute attribute = new CliArgumentAttribute(0, "test");

            // Act & Assert
            string valueName = attribute.ValueName;
            _ = valueName.ShouldNotBeNull();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesZeroPosition()
        {
            // Act
            CliArgumentAttribute attribute = new CliArgumentAttribute(0, "test");

            // Assert
            attribute.Position.ShouldBe(0);
        }

        [Fact]
        public void HandlesLargePosition()
        {
            // Arrange
            int position = 100;

            // Act
            CliArgumentAttribute attribute = new CliArgumentAttribute(position, "test");

            // Assert
            attribute.Position.ShouldBe(position);
        }

        [Fact]
        public void HandlesValueNameWithSpecialCharacters()
        {
            // Arrange
            string valueName = "input_file";

            // Act
            CliArgumentAttribute attribute = new CliArgumentAttribute(0, valueName);

            // Assert
            attribute.ValueName.ShouldBe(valueName);
        }
    }
}

public class CliOptionAttributeTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesAttribute_WithValidTemplate()
        {
            // Arrange
            string expectedTemplate = "--verbose";

            // Act
            CliOptionAttribute attribute = new CliOptionAttribute(expectedTemplate);

            // Assert
            attribute.Template.ShouldBe(expectedTemplate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenTemplateIsNullOrWhiteSpace(string? template)
        {
            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new CliOptionAttribute(template!));
        }
    }

    public class Properties
    {
        [Fact]
        public void TemplateProperty_IsReadOnly()
        {
            // Arrange
            CliOptionAttribute attribute = new CliOptionAttribute("--test");

            // Act & Assert
            string template = attribute.Template;
            _ = template.ShouldNotBeNull();
        }

        [Fact]
        public void IsHiddenProperty_HasDefaultValue()
        {
            // Arrange
            CliOptionAttribute attribute = new CliOptionAttribute("--test");

            // Act & Assert
            attribute.IsHidden.ShouldBeFalse();
        }

        [Fact]
        public void IsHiddenProperty_CanBeSet()
        {
            // Arrange & Act
            CliOptionAttribute attribute = new CliOptionAttribute("--test") { IsHidden = true };

            // Assert
            attribute.IsHidden.ShouldBeTrue();
        }

        [Fact]
        public void IsRequiredProperty_HasDefaultValue()
        {
            // Arrange
            CliOptionAttribute attribute = new CliOptionAttribute("--test");

            // Act & Assert
            attribute.IsRequired.ShouldBeFalse();
        }

        [Fact]
        public void IsRequiredProperty_CanBeSet()
        {
            // Arrange & Act
            CliOptionAttribute attribute = new CliOptionAttribute("--test") { IsRequired = true };

            // Assert
            attribute.IsRequired.ShouldBeTrue();
        }

        [Fact]
        public void ValueIsOptionalProperty_HasDefaultValue()
        {
            // Arrange
            CliOptionAttribute attribute = new CliOptionAttribute("--test");

            // Act & Assert
            attribute.ValueIsOptional.ShouldBeFalse();
        }

        [Fact]
        public void ValueIsOptionalProperty_CanBeSet()
        {
            // Arrange & Act
            CliOptionAttribute attribute = new CliOptionAttribute("--test") { ValueIsOptional = true };

            // Assert
            attribute.ValueIsOptional.ShouldBeTrue();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesShortOption()
        {
            // Arrange
            string template = "-v";

            // Act
            CliOptionAttribute attribute = new CliOptionAttribute(template);

            // Assert
            attribute.Template.ShouldBe(template);
        }

        [Fact]
        public void HandlesLongOption()
        {
            // Arrange
            string template = "--verbose";

            // Act
            CliOptionAttribute attribute = new CliOptionAttribute(template);

            // Assert
            attribute.Template.ShouldBe(template);
        }

        [Fact]
        public void HandlesOptionWithValuePlaceholder()
        {
            // Arrange
            string template = "--output=<file>";

            // Act
            CliOptionAttribute attribute = new CliOptionAttribute(template);

            // Assert
            attribute.Template.ShouldBe(template);
        }

        [Fact]
        public void HandlesMultiplePropertiesSet()
        {
            // Arrange & Act
            CliOptionAttribute attribute = new CliOptionAttribute("--test")
            {
                IsHidden = true,
                IsRequired = true,
                ValueIsOptional = false,
            };

            // Assert
            attribute.IsHidden.ShouldBeTrue();
            attribute.IsRequired.ShouldBeTrue();
            attribute.ValueIsOptional.ShouldBeFalse();
        }
    }
}

public class CliExampleAttributeTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesAttribute_WithSingleArgument()
        {
            // Arrange
            string expectedArgument = "greet John";

            // Act
            CliExampleAttribute attribute = new CliExampleAttribute(expectedArgument);
            _ = attribute; // Suppress IDE0058

            // Assert
            _ = attribute.Arguments.ShouldHaveSingleItem();
            attribute.Arguments[0].ShouldBe(expectedArgument);
        }

        [Fact]
        public void CreatesAttribute_WithMultipleArguments()
        {
            // Arrange
            string[] expectedArguments = new[] { "greet", "John", "--verbose" };

            // Act
            CliExampleAttribute attribute = new CliExampleAttribute(expectedArguments);

            // Assert
            attribute.Arguments.Count.ShouldBe(3);
            attribute.Arguments[0].ShouldBe("greet");
            attribute.Arguments[1].ShouldBe("John");
            attribute.Arguments[2].ShouldBe("--verbose");
        }

        [Fact]
        public void ThrowsArgumentException_WhenNoArgumentsProvided()
        {
            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new CliExampleAttribute(Array.Empty<string>()));
        }
    }

    public class Properties
    {
        [Fact]
        public void ArgumentsProperty_IsReadOnly()
        {
            // Arrange
            CliExampleAttribute attribute = new CliExampleAttribute("test");

            // Act & Assert
            IReadOnlyList<string> arguments = attribute.Arguments;
            _ = arguments.ShouldNotBeNull();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesComplexCommandAsSingleArgument()
        {
            // Arrange
            string command = "greet John --verbose --output=file.txt";

            // Act
            CliExampleAttribute attribute = new CliExampleAttribute(command);
            _ = attribute; // Suppress IDE0058

            // Assert
            _ = attribute.Arguments.ShouldHaveSingleItem();
            attribute.Arguments[0].ShouldBe(command);
        }

        [Fact]
        public void HandlesCommandWithQuotesAsSingleArgument()
        {
            // Arrange
            string command = "greet \"John Doe\"";

            // Act
            CliExampleAttribute attribute = new CliExampleAttribute(command);
            _ = attribute; // Suppress IDE0058

            // Assert
            _ = attribute.Arguments.ShouldHaveSingleItem();
            attribute.Arguments[0].ShouldBe(command);
        }

        [Fact]
        public void HandlesManyArguments()
        {
            // Arrange
            string[] arguments = Enumerable.Range(1, 100).Select(i => $"arg{i}").ToArray();

            // Act
            CliExampleAttribute attribute = new CliExampleAttribute(arguments);

            // Assert
            attribute.Arguments.Count.ShouldBe(100);
        }

        [Fact]
        public void HandlesSpecialCharactersInArguments()
        {
            // Arrange
            string[] arguments = new[] { "command", "--option=value", "path/to/file.txt" };

            // Act
            CliExampleAttribute attribute = new CliExampleAttribute(arguments);

            // Assert
            attribute.Arguments.Count.ShouldBe(3);
            attribute.Arguments[1].ShouldBe("--option=value");
            attribute.Arguments[2].ShouldBe("path/to/file.txt");
        }
    }
}
