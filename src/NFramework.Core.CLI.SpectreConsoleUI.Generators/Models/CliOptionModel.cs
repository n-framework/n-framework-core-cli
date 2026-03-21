namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class CliOptionModel
    {
        public CliOptionModel(string template, bool isHidden, bool isRequired, bool valueIsOptional)
        {
            Template = template;
            IsHidden = isHidden;
            IsRequired = isRequired;
            ValueIsOptional = valueIsOptional;
        }

        public string Template { get; }

        public bool IsHidden { get; }

        public bool IsRequired { get; }

        public bool ValueIsOptional { get; }
    }
}
