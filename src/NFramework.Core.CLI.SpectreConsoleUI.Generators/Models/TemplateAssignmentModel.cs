namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class TemplateAssignmentModel
    {
        public TemplateAssignmentModel(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
