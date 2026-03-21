namespace NFramework.Core.CLI.SpectreConsoleUI.Generators.Templates.Adapter;

internal static partial class AdapterTemplate
{
    public const string Value = """
        file sealed class {{ Command.GeneratedAdapterTypeName }}({{ Command.CommandTypeName }} command) : global::Spectre.Console.Cli.AsyncCommand<{{ Command.GeneratedSettingsTypeName }}>
        {
            private readonly {{ Command.CommandTypeName }} _command = command;

            public override global::System.Threading.Tasks.Task<int> ExecuteAsync(global::Spectre.Console.Cli.CommandContext context, {{ Command.GeneratedSettingsTypeName }} settings, global::System.Threading.CancellationToken cancellationToken)
            {
                string commandName = global::System.String.IsNullOrWhiteSpace(context.Name) || context.Name == {{ Command.DefaultCommandContextNameLiteral }} ? {{ Command.CommandNameLiteral }} : context.Name;
                global::NFramework.Core.CLI.Abstractions.CliCommandContext cliContext = new(commandName, new global::System.Collections.Generic.List<string>(context.Arguments));
                {{ Command.SettingsTypeName }} cliSettings = new()
                {
        {{- for assignment in Command.Assignments }}
                    {{ assignment.PropertyName }} = settings.{{ assignment.PropertyName }}{{ if for.last == false }},{{ end }}
        {{- end }}
                };

                return _command.ExecuteAsync(cliContext, cliSettings, cancellationToken);
            }
        }
        """;
}
