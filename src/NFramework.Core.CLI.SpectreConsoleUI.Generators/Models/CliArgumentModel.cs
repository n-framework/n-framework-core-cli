namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class CliArgumentModel
    {
        public CliArgumentModel(int position, string valueName, bool isRequired)
        {
            Position = position;
            ValueName = valueName;
            IsRequired = isRequired;
        }

        public int Position { get; }

        public string ValueName { get; }

        public bool IsRequired { get; }
    }
}
