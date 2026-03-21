using Microsoft.Extensions.DependencyInjection;
using NFramework.Core.CLI.Abstractions;
using Spectre.Console;

namespace NFramework.Core.CLI.SpectreConsoleUI.DependencyInjection;

public static class SpectreConsoleUiServiceCollectionExtensions
{
    public static IServiceCollection AddCoreCliSpectreConsoleUi(this IServiceCollection services)
    {
        _ = services.AddSingleton<IAnsiConsole>(_ => AnsiConsole.Console);
        _ = services.AddSingleton<ITerminalSession, SpectreConsoleTerminalSession>();

        return services;
    }
}
