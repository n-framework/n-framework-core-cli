namespace NFramework.Core.CLI.SpectreConsoleUI.Generators.Templates.Settings;

internal static partial class SettingsTemplate
{
    public const string Value = """
        file sealed class {{ Command.GeneratedSettingsTypeName }} : global::Spectre.Console.Cli.CommandSettings
        {
        {{- for property in Command.Properties }}
        {{- if property.DescriptionAttribute }}
            {{ property.DescriptionAttribute }}
        {{- end }}
        {{- if property.ArgumentAttribute }}
            {{ property.ArgumentAttribute }}
        {{- end }}
        {{- if property.OptionAttribute }}
            {{ property.OptionAttribute }}
        {{- end }}
            public {{ property.TypeName }} {{ property.Name }} { get; set; }

        {{- end }}
        }
        """;
}
