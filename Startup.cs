using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZohoIntegration.TimeLogs.Repositories;
using ZohoIntegration.TimeLogs.Services;

[assembly: FunctionsStartup(typeof(ZohoIntegration.TimeLogs.Startup))]

namespace ZohoIntegration.TimeLogs;
public class Startup: FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        // builder.Services.AddHttpClient();

        var executionContextOptions = builder.Services.BuildServiceProvider()
            .GetService<IOptions<ExecutionContextOptions>>().Value;

        var functionAppDirectory = executionContextOptions.AppDirectory;

        var config = new ConfigurationBuilder()
            .SetBasePath(functionAppDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        builder.Services.AddScoped(svc => config);

        // repositories
        builder.Services.AddScoped<StorageAccountTableConnection>();
        builder.Services.AddScoped<TimeLogRelation>();
        builder.Services.AddScoped<JobNameRelationRepo>();
        builder.Services.AddScoped<AccessTokenRepo>();
        builder.Services.AddScoped<UsersRepo>();
        builder.Services.AddScoped<UserRelationRepo>();

        // services
        builder.Services.AddScoped<ZohoTimeLogs>();
        builder.Services.AddScoped<ZohoTimesheets>();
        builder.Services.AddScoped<ZohoJobName>();
        builder.Services.AddScoped<ZohoProjects>();
        builder.Services.AddScoped<ZohoConnection>();
        builder.Services.AddScoped<JobName>();
        builder.Services.AddScoped<Users>();
        builder.Services.AddScoped<UserRelation>();
    }
}
