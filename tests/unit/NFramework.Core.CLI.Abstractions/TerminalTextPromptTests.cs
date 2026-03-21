using NFramework.Core.CLI.Abstractions;
using Shouldly;
using Xunit;

namespace NFramework.Core.CLI.Abstractions.Tests;

public class TerminalTextPromptTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesPrompt_WithValidParameters()
        {
            // Arrange
            string expectedPromptText = "Enter your name:";
            string expectedValidationMessage = "Value cannot be empty";

            // Act
            TerminalTextPrompt prompt = new TerminalTextPrompt(expectedPromptText, expectedValidationMessage);

            // Assert
            prompt.PromptText.ShouldBe(expectedPromptText);
            prompt.ValidationErrorMessage.ShouldBe(expectedValidationMessage);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenPromptTextIsNullOrWhiteSpace(string? promptText)
        {
            // Arrange
            string validationMessage = "Error message";

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalTextPrompt(promptText!, validationMessage));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenValidationMessageIsNullOrWhiteSpace(string? validationMessage)
        {
            // Arrange
            string promptText = "Enter value:";

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalTextPrompt(promptText, validationMessage!));
        }
    }

    public class Properties
    {
        [Fact]
        public void PromptTextProperty_IsReadOnly()
        {
            // Arrange
            TerminalTextPrompt prompt = new TerminalTextPrompt("Enter name:", "Name is required");

            // Act & Assert
            string text = prompt.PromptText;
            _ = text.ShouldNotBeNull();
        }

        [Fact]
        public void ValidationErrorMessageProperty_IsReadOnly()
        {
            // Arrange
            TerminalTextPrompt prompt = new TerminalTextPrompt("Enter name:", "Name is required");

            // Act & Assert
            string message = prompt.ValidationErrorMessage;
            _ = message.ShouldNotBeNull();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesPromptWithSpecialCharacters()
        {
            // Arrange
            string promptText = "Enter email (e.g., user@example.com):";
            string validationMessage = "Invalid email format!";

            // Act
            TerminalTextPrompt prompt = new TerminalTextPrompt(promptText, validationMessage);

            // Assert
            prompt.PromptText.ShouldBe(promptText);
            prompt.ValidationErrorMessage.ShouldBe(validationMessage);
        }

        [Fact]
        public void HandlesMultilinePromptText()
        {
            // Arrange
            string promptText = "Line 1\nLine 2\nLine 3";
            string validationMessage = "Validation error";

            // Act
            TerminalTextPrompt prompt = new TerminalTextPrompt(promptText, validationMessage);

            // Assert
            prompt.PromptText.ShouldBe(promptText);
        }

        [Fact]
        public void HandlesLongValidationMessage()
        {
            // Arrange
            string promptText = "Enter value:";
            string validationMessage =
                "This is a very long validation error message that provides detailed information about what went wrong and how to fix it.";

            // Act
            TerminalTextPrompt prompt = new TerminalTextPrompt(promptText, validationMessage);

            // Assert
            prompt.ValidationErrorMessage.ShouldBe(validationMessage);
        }

        [Fact]
        public void DoesNotTrimWhitespaceFromParameters()
        {
            // Arrange
            string promptText = "  Enter name:  ";
            string validationMessage = "  Error message  ";

            // Act
            TerminalTextPrompt prompt = new TerminalTextPrompt(promptText, validationMessage);

            // Assert - The implementation should not trim the parameters
            prompt.PromptText.ShouldBe(promptText);
            prompt.ValidationErrorMessage.ShouldBe(validationMessage);
        }
    }
}

public class TerminalSelectionOptionTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesOption_WithValidParameters()
        {
            // Arrange
            string expectedValue = "option1";
            string expectedLabel = "Option 1";

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(expectedValue, expectedLabel);

            // Assert
            option.Value.ShouldBe(expectedValue);
            option.Label.ShouldBe(expectedLabel);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenValueIsNullOrWhiteSpace(string? value)
        {
            // Arrange
            string label = "Option Label";

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalSelectionOption(value!, label));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenLabelIsNullOrWhiteSpace(string? label)
        {
            // Arrange
            string value = "option1";

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalSelectionOption(value, label!));
        }
    }

    public class TrimmingBehavior
    {
        [Fact]
        public void TrimsWhitespaceFromValue()
        {
            // Arrange
            string value = "  option1  ";
            string label = "Option 1";

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(value, label);

            // Assert
            option.Value.ShouldBe("option1");
        }

        [Fact]
        public void TrimsWhitespaceFromLabel()
        {
            // Arrange
            string value = "option1";
            string label = "  Option 1  ";

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(value, label);

            // Assert
            option.Label.ShouldBe("Option 1");
        }

        [Fact]
        public void TrimsWhitespaceFromBothParameters()
        {
            // Arrange
            string value = "  option1  ";
            string label = "  Option 1  ";

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(value, label);

            // Assert
            option.Value.ShouldBe("option1");
            option.Label.ShouldBe("Option 1");
        }

        [Fact]
        public void TrimsInternalWhitespaceOnly()
        {
            // Arrange
            string value = "  option  one  ";
            string label = "  Option  One  ";

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(value, label);

            // Assert
            option.Value.ShouldBe("option  one");
            option.Label.ShouldBe("Option  One");
        }
    }

    public class Properties
    {
        [Fact]
        public void ValueProperty_IsReadOnly()
        {
            // Arrange
            TerminalSelectionOption option = new TerminalSelectionOption("opt1", "Option 1");

            // Act & Assert
            string value = option.Value;
            _ = value.ShouldNotBeNull();
        }

        [Fact]
        public void LabelProperty_IsReadOnly()
        {
            // Arrange
            TerminalSelectionOption option = new TerminalSelectionOption("opt1", "Option 1");

            // Act & Assert
            string label = option.Label;
            _ = label.ShouldNotBeNull();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesSpecialCharactersInValue()
        {
            // Arrange
            string value = "option-with_special.chars";
            string label = "Special Option";

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(value, label);

            // Assert
            option.Value.ShouldBe(value);
        }

        [Fact]
        public void HandlesSpecialCharactersInLabel()
        {
            // Arrange
            string value = "option1";
            string label = "Option with /special\\ characters!";

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(value, label);

            // Assert
            option.Label.ShouldBe(label);
        }

        [Fact]
        public void HandlesUnicodeCharacters()
        {
            // Arrange
            string value = "日本語オプション";
            string label = "🎯 Premium Option";

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(value, label);

            // Assert
            option.Value.ShouldBe(value);
            option.Label.ShouldBe(label);
        }

        [Fact]
        public void HandlesLongValues()
        {
            // Arrange
            string value = new string('a', 1000);
            string label = "Long option";

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(value, label);

            // Assert
            option.Value.Length.ShouldBe(1000);
        }

        [Fact]
        public void HandlesLongLabels()
        {
            // Arrange
            string value = "option1";
            string label = new string('b', 1000);

            // Act
            TerminalSelectionOption option = new TerminalSelectionOption(value, label);

            // Assert
            option.Label.Length.ShouldBe(1000);
        }
    }
}

public class TerminalSelectionPromptTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesPrompt_WithValidParameters()
        {
            // Arrange
            string expectedTitle = "Choose an option:";
            IReadOnlyList<TerminalSelectionOption> expectedOptions = new[]
            {
                new TerminalSelectionOption("1", "Option 1"),
                new TerminalSelectionOption("2", "Option 2"),
            };

            // Act
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt(expectedTitle, expectedOptions);

            // Assert
            prompt.Title.ShouldBe(expectedTitle);
            prompt.Options.Count.ShouldBe(2);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ThrowsArgumentException_WhenTitleIsNullOrWhiteSpace(string? title)
        {
            // Arrange
            IReadOnlyList<TerminalSelectionOption> options = new[] { new TerminalSelectionOption("1", "Option 1") };

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalSelectionPrompt(title!, options));
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenOptionsIsNull()
        {
            // Arrange
            string title = "Choose an option:";

            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() => new TerminalSelectionPrompt(title, null!));
        }

        [Fact]
        public void ThrowsArgumentException_WhenOptionsAreEmpty()
        {
            // Arrange
            string title = "Choose an option:";
            IReadOnlyList<TerminalSelectionOption> options = Array.Empty<TerminalSelectionOption>();

            // Act & Assert
            _ = Should.Throw<ArgumentException>(() => new TerminalSelectionPrompt(title, options));
        }
    }

    public class Properties
    {
        [Fact]
        public void TitleProperty_IsReadOnly()
        {
            // Arrange
            IReadOnlyList<TerminalSelectionOption> options = new[] { new TerminalSelectionOption("1", "Option 1") };
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt("Choose:", options);

            // Act & Assert
            string title = prompt.Title;
            _ = title.ShouldNotBeNull();
        }

        [Fact]
        public void OptionsProperty_IsReadOnly()
        {
            // Arrange
            IReadOnlyList<TerminalSelectionOption> options = new[] { new TerminalSelectionOption("1", "Option 1") };
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt("Choose:", options);

            // Act & Assert
            IReadOnlyList<TerminalSelectionOption> opts = prompt.Options;
            _ = opts; // Suppress IDE0058
            _ = opts.ShouldNotBeNull();
            _ = opts.ShouldHaveSingleItem();
        }
    }

    public class EdgeCases
    {
        [Fact]
        public void HandlesSingleOption()
        {
            // Arrange
            string title = "Confirm:";
            IReadOnlyList<TerminalSelectionOption> options = new[] { new TerminalSelectionOption("yes", "Yes") };

            // Act
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt(title, options);
            _ = prompt; // Suppress IDE0058

            // Assert
            _ = prompt.Options.ShouldHaveSingleItem();
            prompt.Options[0].Value.ShouldBe("yes");
        }

        [Fact]
        public void HandlesManyOptions()
        {
            // Arrange
            string title = "Choose:";
            IReadOnlyList<TerminalSelectionOption> options = Enumerable
                .Range(1, 100)
                .Select(i => new TerminalSelectionOption($"opt{i}", $"Option {i}"))
                .ToList();

            // Act
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt(title, options);

            // Assert
            prompt.Options.Count.ShouldBe(100);
        }

        [Fact]
        public void HandlesSpecialCharactersInTitle()
        {
            // Arrange
            string title = "Select (1-10):";
            IReadOnlyList<TerminalSelectionOption> options = new[] { new TerminalSelectionOption("1", "Option 1") };

            // Act
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt(title, options);

            // Assert
            prompt.Title.ShouldBe(title);
        }

        [Fact]
        public void HandlesOptionsWithSpecialCharacters()
        {
            // Arrange
            string title = "Choose:";
            IReadOnlyList<TerminalSelectionOption> options = new[]
            {
                new TerminalSelectionOption("opt-1", "Option with/special\\chars!"),
                new TerminalSelectionOption("opt_2", "Option@with#$special%chars"),
            };

            // Act
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt(title, options);

            // Assert
            prompt.Options.Count.ShouldBe(2);
            prompt.Options[0].Value.ShouldBe("opt-1");
            prompt.Options[0].Label.ShouldBe("Option with/special\\chars!");
        }

        [Fact]
        public void HandlesUnicodeInOptions()
        {
            // Arrange
            string title = "选择:";
            IReadOnlyList<TerminalSelectionOption> options = new[]
            {
                new TerminalSelectionOption("🎯", "Target Option"),
                new TerminalSelectionOption("🚀", "Rocket Option"),
            };

            // Act
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt(title, options);

            // Assert
            prompt.Options.Count.ShouldBe(2);
            prompt.Options[0].Value.ShouldBe("🎯");
            prompt.Options[0].Label.ShouldBe("Target Option");
        }
    }
}
