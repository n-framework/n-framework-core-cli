using System.Collections.Immutable;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class TemplateSettingsCommandModel
    {
        public TemplateSettingsCommandModel(
            string generatedSettingsTypeName,
            ImmutableArray<TemplatePropertyModel> properties
        )
        {
            GeneratedSettingsTypeName = generatedSettingsTypeName;
            Properties = properties;
        }

        public string GeneratedSettingsTypeName { get; }

        public ImmutableArray<TemplatePropertyModel> Properties { get; }
    }
}
