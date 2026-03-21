namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class TemplateExampleModel
    {
        public TemplateExampleModel(string argumentsLiteral)
        {
            ArgumentsLiteral = argumentsLiteral;
        }

        public string ArgumentsLiteral { get; }
    }
}
