namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class TemplatePropertyModel
    {
        public TemplatePropertyModel(
            string? descriptionAttribute,
            string? argumentAttribute,
            string? optionAttribute,
            string typeName,
            string name
        )
        {
            DescriptionAttribute = descriptionAttribute;
            ArgumentAttribute = argumentAttribute;
            OptionAttribute = optionAttribute;
            TypeName = typeName;
            Name = name;
        }

        public string? DescriptionAttribute { get; }

        public string? ArgumentAttribute { get; }

        public string? OptionAttribute { get; }

        public string TypeName { get; }

        public string Name { get; }
    }
}
