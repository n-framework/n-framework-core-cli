using Microsoft.Extensions.DependencyInjection;
using NFramework.Core.CLI.Abstractions;
using NFramework.Core.CLI.SpectreConsoleUI.DependencyInjection;
using Shouldly;
using Spectre.Console;
using Xunit;

namespace NFramework.Core.CLI.SpectreConsoleUI.Tests.DependencyInjection;

public class SpectreConsoleUiServiceCollectionExtensionsTests
{
    public class AddCoreCliSpectreConsoleUi
    {
        [Fact]
        public void ReturnsSameServiceCollection_Instance()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            IServiceCollection result = services.AddCoreCliSpectreConsoleUi();

            // Assert
            services.ShouldBe(result);
        }

        [Fact]
        public void RegistersIAnsiConsole_AsSingleton()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            _ = services.AddCoreCliSpectreConsoleUi();

            // Assert
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            IAnsiConsole console1 = serviceProvider.GetRequiredService<IAnsiConsole>();
            IAnsiConsole console2 = serviceProvider.GetRequiredService<IAnsiConsole>();

            console1.ShouldBe(console2);
        }

        [Fact]
        public void RegistersITerminalSession_AsSingleton()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            _ = services.AddCoreCliSpectreConsoleUi();

            // Assert
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ITerminalSession session1 = serviceProvider.GetRequiredService<ITerminalSession>();
            ITerminalSession session2 = serviceProvider.GetRequiredService<ITerminalSession>();

            session1.ShouldBe(session2);
        }

        [Fact]
        public void ITerminalSession_IsSpectreConsoleTerminalSession()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            _ = services.AddCoreCliSpectreConsoleUi();

            // Assert
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ITerminalSession session = serviceProvider.GetRequiredService<ITerminalSession>();

            _ = session.ShouldBeOfType<SpectreConsoleTerminalSession>();
        }

        [Fact]
        public void AllowsChainingWithOtherServices()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            IServiceCollection result = services.AddCoreCliSpectreConsoleUi().AddSingleton<string>("test");

            // Assert
            services.ShouldBe(result);
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            string testService = serviceProvider.GetRequiredService<string>();
            testService.ShouldBe("test");
        }

        [Fact]
        public void CanBeCalledMultipleTimes_SameCollection()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            _ = services.AddCoreCliSpectreConsoleUi();
            _ = services.AddCoreCliSpectreConsoleUi();

            // Assert - Should not throw, services are registered as singletons so duplicate calls are safe
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ITerminalSession session = serviceProvider.GetRequiredService<ITerminalSession>();
            _ = session.ShouldNotBeNull();
        }
    }
}
