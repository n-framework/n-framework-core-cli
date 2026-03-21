namespace NFramework.Core.CLI.Abstractions;

public interface IAsyncCliCommand<in TSettings>
{
    Task<int> ExecuteAsync(CliCommandContext context, TSettings settings, CancellationToken cancellationToken);
}
