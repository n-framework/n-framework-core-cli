namespace NFramework.Core.CLI.Abstractions;

public sealed class TerminalTextPrompt
{
    public TerminalTextPrompt(string promptText, string validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(promptText))
            throw new ArgumentException("Prompt text cannot be empty or whitespace.", nameof(promptText));

        if (string.IsNullOrWhiteSpace(validationErrorMessage))
            throw new ArgumentException(
                "Validation error message cannot be empty or whitespace.",
                nameof(validationErrorMessage)
            );

        PromptText = promptText;
        ValidationErrorMessage = validationErrorMessage;
    }

    public string PromptText { get; }

    public string ValidationErrorMessage { get; }
}
