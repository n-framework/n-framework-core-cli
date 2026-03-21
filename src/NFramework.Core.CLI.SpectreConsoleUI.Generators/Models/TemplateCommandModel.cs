using System.Collections.Immutable;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class TemplateCommandModel
    {
        public TemplateCommandModel(
            string commandTypeName,
            string generatedAdapterTypeName,
            string configuratorName,
            string commandNameLiteral,
            string descriptionLiteral,
            ImmutableArray<TemplateExampleModel> examples,
            string generatedSettingsSource,
            string generatedAdapterSource
        )
        {
            CommandTypeName = commandTypeName;
            GeneratedAdapterTypeName = generatedAdapterTypeName;
            ConfiguratorName = configuratorName;
            CommandNameLiteral = commandNameLiteral;
            DescriptionLiteral = descriptionLiteral;
            Examples = examples;
            GeneratedSettingsSource = generatedSettingsSource;
            GeneratedAdapterSource = generatedAdapterSource;
        }

        public string CommandTypeName { get; }

        public string GeneratedAdapterTypeName { get; }

        public string ConfiguratorName { get; }

        public string CommandNameLiteral { get; }

        public string DescriptionLiteral { get; }

        public ImmutableArray<TemplateExampleModel> Examples { get; }

        public string GeneratedSettingsSource { get; }

        public string GeneratedAdapterSource { get; }
    }
}
