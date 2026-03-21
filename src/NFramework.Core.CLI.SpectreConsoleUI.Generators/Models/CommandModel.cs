using System.Collections.Immutable;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class CommandModel
    {
        public CommandModel(
            string commandTypeName,
            string commandTypeSimpleName,
            string settingsTypeName,
            string settingsTypeSimpleName,
            string commandName,
            string description,
            ImmutableArray<ExampleModel> examples,
            ImmutableArray<PropertyModel> properties
        )
        {
            CommandTypeName = commandTypeName;
            CommandTypeSimpleName = commandTypeSimpleName;
            SettingsTypeName = settingsTypeName;
            SettingsTypeSimpleName = settingsTypeSimpleName;
            CommandName = commandName;
            Description = description;
            Examples = examples;
            Properties = properties;
        }

        public string CommandTypeName { get; }

        public string CommandTypeSimpleName { get; }

        public string SettingsTypeName { get; }

        public string SettingsTypeSimpleName { get; }

        public string CommandName { get; }

        public string Description { get; }

        public ImmutableArray<ExampleModel> Examples { get; }

        public ImmutableArray<PropertyModel> Properties { get; }

        public string GeneratedSettingsTypeName => "Generated_" + sanitize(CommandTypeSimpleName) + "_SpectreSettings";

        public string GeneratedAdapterTypeName => "Generated_" + sanitize(CommandTypeSimpleName) + "_SpectreAdapter";
    }
}
