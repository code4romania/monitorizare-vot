using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using VoteMonitor.Api.Extensions;
using VoteMonitor.Api.Extensions.HealthChecks;
using Xunit;

namespace VoteMonitor.Api.Tests.Extensions;

public class ServiceExtensions_ConditionalChecksRegistrationsTests
{
    [Theory]
    [InlineData("VoteMonitorContext")]
    [InlineData("Redis")]
    [InlineData("AzureBlobStorage")]
    [InlineData("Firebase")]
    public void RegistersRequiredChecks(string registrationName)
    {
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        serviceCollection.AddHealthChecks(configuration);

        var sp = serviceCollection.BuildServiceProvider();
        var registrations = sp.GetService<IOptions<HealthCheckServiceOptions>>().Value.Registrations;
        registrations.Should().Contain(c => c.Name == registrationName);
    }

    [Fact]
    public async Task WhenApplicationCacheOptionsImplementationIsRedis_ThenRedisIsChecked()
    {
        // Arrange
        var healthCheckMock = new Mock<IHealthCheck>();
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "EnableHealthChecks", "true"},
                { "ApplicationCacheOptions:Implementation", "RedisCache"}
            })
            .Build();
        var (registration, conditionalHealthCheck) = ArrangeRegistration(healthCheckMock, serviceCollection, configuration, "Redis");

        // Act
        await conditionalHealthCheck.CheckHealthAsync(new HealthCheckContext
        {
            Registration = registration
        });

        // Assert
        healthCheckMock.Verify(c => c.CheckHealthAsync(It.IsAny<HealthCheckContext>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task WhenApplicationCacheOptionsImplementationIsNotRedis_ThenRedisIsNotChecked()
    {
        // Arrange
        var healthCheckMock = new Mock<IHealthCheck>();
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .Build();
        var (registration, conditionalHealthCheck) = ArrangeRegistration(healthCheckMock, serviceCollection, configuration, "Redis");

        // Act
        await conditionalHealthCheck.CheckHealthAsync(new HealthCheckContext
        {
            Registration = registration
        });

        // Assert
        healthCheckMock.Verify(c => c.CheckHealthAsync(It.IsAny<HealthCheckContext>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task WhenFirebaseConnectionIsNotSet_ThenFirebaseIsNotChecked()
    {
        // Arrange
        var healthCheckMock = new Mock<IHealthCheck>();
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var (registration, conditionalHealthCheck) = ArrangeRegistration(healthCheckMock, serviceCollection, configuration, "Firebase");

        // Act
        await conditionalHealthCheck.CheckHealthAsync(new HealthCheckContext
        {
            Registration = registration
        });

        // Assert
        healthCheckMock.Verify(c => c.CheckHealthAsync(It.IsAny<HealthCheckContext>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task WhenFirebaseConnectionIsSet_ThenFirebaseIsChecked()
    {
        // Arrange
        var healthCheckMock = new Mock<IHealthCheck>();
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "EnableHealthChecks", "true"},
                { "FirebaseServiceOptions:ServerKey", "a valid server key"}
            })
            .Build();
        var (registration, conditionalHealthCheck) = ArrangeRegistration(healthCheckMock, serviceCollection, configuration, "Firebase");

        // Act
        await conditionalHealthCheck.CheckHealthAsync(new HealthCheckContext
        {
            Registration = registration
        });

        // Assert
        healthCheckMock.Verify(c => c.CheckHealthAsync(It.IsAny<HealthCheckContext>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task WhenFileServiceLocalFileService_ThenAzureBlobStorageIsNotChecked()
    {
        // Arrange
        var healthCheckMock = new Mock<IHealthCheck>();
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "EnableHealthChecks", "true"},
                { "FileServiceOptions:Type", "LocalFileService"}
            })
            .Build();
        var (registration, conditionalHealthCheck) = ArrangeRegistration(healthCheckMock, serviceCollection, configuration, "AzureBlobStorage");

        // Act
        await conditionalHealthCheck.CheckHealthAsync(new HealthCheckContext
        {
            Registration = registration
        });

        // Assert
        healthCheckMock.Verify(c => c.CheckHealthAsync(It.IsAny<HealthCheckContext>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task WhenFileServiceNotLocalFileService_ThenAzureBlobStorageIsChecked()
    {
        // Arrange
        var healthCheckMock = new Mock<IHealthCheck>();
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "EnableHealthChecks", "true"},
                { "FileServiceOptions:Type", "BlobService"}
            })
            .Build();
        var (registration, conditionalHealthCheck) = ArrangeRegistration(healthCheckMock, serviceCollection, configuration, "AzureBlobStorage");

        // Act
        await conditionalHealthCheck.CheckHealthAsync(new HealthCheckContext
        {
            Registration = registration
        });

        // Assert
        healthCheckMock.Verify(c => c.CheckHealthAsync(It.IsAny<HealthCheckContext>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    private static (HealthCheckRegistration registration, ConditionalHealthCheck healthCheck) ArrangeRegistration(Mock<IHealthCheck> healthCheckMock, ServiceCollection serviceCollection, IConfigurationRoot configuration, string name)
    {
        serviceCollection.AddTransient<ILogger<ConditionalHealthCheck>>(_ => NullLogger<ConditionalHealthCheck>.Instance);
        serviceCollection.AddHealthChecks(configuration);

        var sp = serviceCollection.BuildServiceProvider();
        var registrations = sp.GetService<IOptions<HealthCheckServiceOptions>>().Value.Registrations;
        var registration = registrations.First(c => c.Name == name);
        var healthCheck = registration.Factory(sp);
        healthCheck.Should().BeOfType(typeof(ConditionalHealthCheck));

        var conditionalHealthCheck = healthCheck as ConditionalHealthCheck;
        conditionalHealthCheck.HealthCheckFactory = () => healthCheckMock.Object;

        return (registration, conditionalHealthCheck);
    }
}
