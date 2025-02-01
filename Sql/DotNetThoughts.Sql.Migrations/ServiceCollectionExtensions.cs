using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace DotNetThoughts.Sql.Migrations;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddMigrationRunner<TMigrationRunnerImplementation>(
        this IHostApplicationBuilder applicationBuilder,
        string configurationSectionName)
        where TMigrationRunnerImplementation : MigrationRunner<TMigrationRunnerImplementation>
    {
        applicationBuilder.Services.AddMigrationRunner<TMigrationRunnerImplementation>(applicationBuilder.Configuration, configurationSectionName);
        return applicationBuilder;
    }

    public static IHostApplicationBuilder AddMigrationRunner<TMigrationRunnerImplementation>(
        this IHostApplicationBuilder applicationBuilder,
        Action<MigrationRunnerOptions<TMigrationRunnerImplementation>> configureOptions)
        where TMigrationRunnerImplementation : MigrationRunner<TMigrationRunnerImplementation>
    {
        applicationBuilder.Services.AddMigrationRunner(configureOptions);
        return applicationBuilder;
    }

    public static IHostApplicationBuilder AddMigrationRunner<TMigrationRunnerImplementation>(
       this IHostApplicationBuilder applicationBuilder,
       string configurationSectionName,
       Action<MigrationRunnerOptions<TMigrationRunnerImplementation>> configureOptions)
       where TMigrationRunnerImplementation : MigrationRunner<TMigrationRunnerImplementation>
    {
        applicationBuilder.Services.AddMigrationRunner(applicationBuilder.Configuration, configurationSectionName, configureOptions);
        return applicationBuilder;
    }


    public static IServiceCollection AddMigrationRunner<TMigrationRunnerImplementation>(
       this IServiceCollection serviceCollection,
       IConfiguration configuration,
       string configurationSectionName)
       where TMigrationRunnerImplementation : MigrationRunner<TMigrationRunnerImplementation>
    {
        serviceCollection.AddSingleton<TMigrationRunnerImplementation>();
        serviceCollection.AddSingleton<MigrationRunnerConfiguration<TMigrationRunnerImplementation>>();
        serviceCollection.AddOptions<MigrationRunnerOptions<TMigrationRunnerImplementation>>()
            .Bind(configuration.GetSection(configurationSectionName));
        return serviceCollection;
    }

    public static IServiceCollection AddMigrationRunner<TMigrationRunnerImplementation>(
       this IServiceCollection serviceCollection,
       Action<MigrationRunnerOptions<TMigrationRunnerImplementation>> configureOptions)
       where TMigrationRunnerImplementation : MigrationRunner<TMigrationRunnerImplementation>
    {
        serviceCollection.AddSingleton<TMigrationRunnerImplementation>();
        serviceCollection.AddSingleton<MigrationRunnerConfiguration<TMigrationRunnerImplementation>>();
        serviceCollection.AddOptions<MigrationRunnerOptions<TMigrationRunnerImplementation>>()
            .Configure(configureOptions);
        return serviceCollection;
    }

    public static IServiceCollection AddMigrationRunner<TMigrationRunnerImplementation>(
      this IServiceCollection serviceCollection,
      IConfiguration configuration,
      string configurationSectionName,
      Action<MigrationRunnerOptions<TMigrationRunnerImplementation>> configureOptions)
      where TMigrationRunnerImplementation : MigrationRunner<TMigrationRunnerImplementation>
    {
        serviceCollection.AddSingleton<TMigrationRunnerImplementation>();
        serviceCollection.AddSingleton<MigrationRunnerConfiguration<TMigrationRunnerImplementation>>();
        serviceCollection.AddOptions<MigrationRunnerOptions<TMigrationRunnerImplementation>>()
            .Bind(configuration.GetSection(configurationSectionName))
            .Configure(configureOptions);
        return serviceCollection;
    }
}