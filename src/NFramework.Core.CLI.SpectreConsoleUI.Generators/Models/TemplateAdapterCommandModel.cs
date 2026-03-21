using System.Collections.Immutable;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class TemplateAdapterCommandModel
    {
        public TemplateAdapterCommandModel(
            string generatedAdapterTypeName,
            string commandTypeName,
            string generatedSettingsTypeName,
            string settingsTypeName,
            string commandNameLiteral,
            string defaultCommandContextNameLiteral,
            ImmutableArray<TemplateAssignmentModel> assignments
        )
        {
            GeneratedAdapterTypeName = generatedAdapterTypeName;
            CommandTypeName = commandTypeName;
            GeneratedSettingsTypeName = generatedSettingsTypeName;
            SettingsTypeName = settingsTypeName;
            CommandNameLiteral = commandNameLiteral;
            DefaultCommandContextNameLiteral = defaultCommandContextNameLiteral;
            Assignments = assignments;
        }

        public string GeneratedAdapterTypeName { get; }

        public string CommandTypeName { get; }

        public string GeneratedSettingsTypeName { get; }

        public string SettingsTypeName { get; }

        public string CommandNameLiteral { get; }

        public string DefaultCommandContextNameLiteral { get; }

        public ImmutableArray<TemplateAssignmentModel> Assignments { get; }
    }
}
