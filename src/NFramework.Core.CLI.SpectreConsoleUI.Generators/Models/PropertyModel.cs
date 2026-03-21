namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class PropertyModel
    {
        public PropertyModel(
            string name,
            string typeName,
            string? description,
            CliArgumentModel? argument,
            CliOptionModel? option
        )
        {
            Name = name;
            TypeName = typeName;
            Description = description;
            Argument = argument;
            Option = option;
        }

        public string Name { get; }

        public string TypeName { get; }

        public string? Description { get; }

        public CliArgumentModel? Argument { get; }

        public CliOptionModel? Option { get; }
    }
}
