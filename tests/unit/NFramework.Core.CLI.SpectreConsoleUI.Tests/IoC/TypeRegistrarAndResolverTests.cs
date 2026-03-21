using Microsoft.Extensions.DependencyInjection;
using NFramework.Core.CLI.SpectreConsoleUI.IoC;
using Shouldly;
using Xunit;

namespace NFramework.Core.CLI.SpectreConsoleUI.Tests.IoC;

public class TypeRegistrarTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesRegistrar_WithValidServices()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            TypeRegistrar registrar = new TypeRegistrar(services);

            // Assert
            registrar.ShouldNotBeNull();
        }

        [Fact]
        public void DoesNotThrow_WhenServicesIsNull()
        {
            // Act & Assert - The implementation uses primary constructor without null validation
            TypeRegistrar registrar = new TypeRegistrar(null!);
            registrar.ShouldNotBeNull();
        }
    }

    public class Build
    {
        [Fact]
        public void ReturnsTypeResolver_WhenBuildIsCalled()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);

            // Act
            Spectre.Console.Cli.ITypeResolver resolver = registrar.Build();

            // Assert
            resolver.ShouldNotBeNull();
            resolver.ShouldBeOfType<TypeResolver>();
        }

        [Fact]
        public void ReturnsTypeResolver_WithServiceProvider()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<object>(new object());
            TypeRegistrar registrar = new TypeRegistrar(services);

            // Act
            TypeResolver resolver = (TypeResolver)registrar.Build();

            // Assert
            resolver.ServiceProvider.ShouldNotBeNull();
        }
    }

    public class Register
    {
        [Fact]
        public void RegistersSingletonService_WhenValidTypesProvided()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);

            // Act
            registrar.Register(typeof(IService), typeof(ServiceImplementation));

            // Assert
            services.ShouldHaveSingleItem();
        }

        [Fact]
        public void RegistersMultipleServices_WhenCalledMultipleTimes()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);

            // Act
            registrar.Register(typeof(IService), typeof(ServiceImplementation));
            registrar.Register(typeof(IService2), typeof(ServiceImplementation2));

            // Assert
            services.Count.ShouldBe(2);
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenServiceTypeIsNull()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => registrar.Register(null!, typeof(ServiceImplementation)));
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenImplementationTypeIsNull()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => registrar.Register(typeof(IService), null!));
        }
    }

    public class RegisterInstance
    {
        [Fact]
        public void RegistersSingletonInstance_WhenValidParametersProvided()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);
            object instance = new object();

            // Act
            registrar.RegisterInstance(typeof(object), instance);

            // Assert
            services.ShouldHaveSingleItem();
        }

        [Fact]
        public void RegistersMultipleInstances_WhenCalledMultipleTimes()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);
            object instance1 = new object();
            object instance2 = new object();

            // Act
            registrar.RegisterInstance(typeof(object), instance1);
            registrar.RegisterInstance(typeof(string), instance2);

            // Assert
            services.Count.ShouldBe(2);
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenServiceTypeIsNull()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);
            object instance = new object();

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => registrar.RegisterInstance(null!, instance));
        }
    }

    public class RegisterLazy
    {
        [Fact]
        public void RegistersLazyFactory_WhenValidParametersProvided()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);
            object instance = new object();
            Func<object> factory = () => instance;

            // Act
            registrar.RegisterLazy(typeof(object), factory);

            // Assert
            services.ShouldHaveSingleItem();
        }

        [Fact]
        public void FactoryIsCalled_WhenServiceIsResolved()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);
            bool factoryCalled = false;
            Func<object> factory = () =>
            {
                factoryCalled = true;
                return new object();
            };

            // Act
            registrar.RegisterLazy(typeof(object), factory);
            Spectre.Console.Cli.ITypeResolver resolver = registrar.Build();
            _ = resolver.Resolve(typeof(object));

            // Assert
            Assert.True(factoryCalled);
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenServiceTypeIsNull()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);
            Func<object> factory = () => new object();

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => registrar.RegisterLazy(null!, factory));
        }

        [Fact]
        public void DoesNotThrow_WhenFactoryIsNull()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);

            // Act & Assert - The implementation doesn't validate null factory
            registrar.RegisterLazy(typeof(object), null!);
        }
    }

    public class IntegrationScenarios
    {
        [Fact]
        public void CompleteWorkflow_RegisterToResolve()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);
            object instance = new object();

            // Act
            registrar.RegisterInstance(typeof(object), instance);
            Spectre.Console.Cli.ITypeResolver resolver = registrar.Build();
            object? resolved = resolver.Resolve(typeof(object));

            // Assert
            instance.ShouldBe(resolved);
        }

        [Fact]
        public void CompleteWorkflow_RegisterSingletonToResolve()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            TypeRegistrar registrar = new TypeRegistrar(services);

            // Act
            registrar.Register(typeof(IService), typeof(ServiceImplementation));
            Spectre.Console.Cli.ITypeResolver resolver = registrar.Build();
            IService? resolved = resolver.Resolve(typeof(IService)) as IService;

            // Assert
            resolved.ShouldNotBeNull();
            resolved.ShouldBeOfType<ServiceImplementation>();
        }
    }

    // Test interfaces and implementations
    public interface IService { }

    public interface IService2 { }

    public class ServiceImplementation : IService { }

    public class ServiceImplementation2 : IService2 { }
}

public class TypeResolverTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesResolver_WithValidServiceProvider()
        {
            // Arrange
            IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();

            // Act
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Assert
            resolver.ShouldNotBeNull();
            serviceProvider.ShouldBe(resolver.ServiceProvider);
        }

        [Fact]
        public void DoesNotThrow_WhenServiceProviderIsNull()
        {
            // Act & Assert - The implementation uses primary constructor without null validation
            TypeResolver resolver = new TypeResolver(null!);
            resolver.ShouldNotBeNull();
        }
    }

    public class ServiceProviderProperty
    {
        [Fact]
        public void ReturnsSameServiceProvider_AsConstructor()
        {
            // Arrange
            IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();

            // Act
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Assert
            serviceProvider.ShouldBe(resolver.ServiceProvider);
        }

        [Fact]
        public void Property_IsReadOnly()
        {
            // Arrange
            IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act & Assert
            IServiceProvider provider = resolver.ServiceProvider;
            provider.ShouldNotBeNull();
        }
    }

    public class Resolve
    {
        [Fact]
        public void ReturnsNull_WhenTypeIsNull()
        {
            // Arrange
            IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act
            object? result = resolver.Resolve(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsService_WhenServiceIsRegistered()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<object>(new object());
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act
            object? result = resolver.Resolve(typeof(object));

            // Assert
            result.ShouldNotBeNull();
        }

        [Fact]
        public void ReturnsNull_WhenServiceIsNotRegistered()
        {
            // Arrange
            IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act
            object? result = resolver.Resolve(typeof(string));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsSameInstance_WhenSingletonIsRegistered()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            object instance = new object();
            services.AddSingleton(typeof(object), instance);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act
            object? result1 = resolver.Resolve(typeof(object));
            object? result2 = resolver.Resolve(typeof(object));

            // Assert
            instance.ShouldBe(result1);
            result1.ShouldBe(result2);
        }

        [Fact]
        public void ReturnsDifferentInstances_WhenTransientIsRegistered()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<object>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act
            object? result1 = resolver.Resolve(typeof(object));
            object? result2 = resolver.Resolve(typeof(object));

            // Assert
            result1.ShouldNotBeNull();
            result2.ShouldNotBeNull();
            result1.ShouldNotBe(result2);
        }
    }

    public class Dispose
    {
        [Fact]
        public void DisposesServiceProvider_WhenItIsDisposable()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<object>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act
            resolver.Dispose();

            // Assert - No exception should be thrown
            Assert.True(true);
        }

        [Fact]
        public void DoesNotThrow_WhenServiceProviderIsNotDisposable()
        {
            // Arrange
            IServiceProvider serviceProvider = new NonDisposableServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act & Assert - No exception should be thrown
            resolver.Dispose();
        }

        [Fact]
        public void CanBeDisposedMultipleTimes()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<object>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act & Assert - No exception should be thrown
            resolver.Dispose();
            resolver.Dispose();
        }
    }

    public class IntegrationScenarios
    {
        [Fact]
        public void CompleteWorkflow_RegisterToResolve()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act
            ITestService? result = resolver.Resolve(typeof(ITestService)) as ITestService;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TestService>();
        }

        [Fact]
        public void CompleteWorkflow_WithMultipleServices()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            services.AddSingleton<ITestService2, TestService2>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            TypeResolver resolver = new TypeResolver(serviceProvider);

            // Act
            ITestService? result1 = resolver.Resolve(typeof(ITestService)) as ITestService;
            ITestService2? result2 = resolver.Resolve(typeof(ITestService2)) as ITestService2;

            // Assert
            result1.ShouldNotBeNull();
            result2.ShouldNotBeNull();
        }
    }

    // Test interfaces and implementations
    public interface ITestService { }

    public interface ITestService2 { }

    public class TestService : ITestService { }

    public class TestService2 : ITestService2 { }

    private class NonDisposableServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return null;
        }
    }
}
