namespace NFramework.Core.CLI.SpectreConsoleUI.Generators.Templates.Application;

public static partial class ApplicationTemplate
{
    internal sealed class Model
    {
        public string? NamespaceName { get; set; }

        public bool HasNamespace { get; set; }

        public string? ApplicationTypeName { get; set; }

        public string? ApplicationNameLiteral { get; set; }

        public bool ValidateExamples { get; set; }

        public object? Commands { get; set; }
    }
}
