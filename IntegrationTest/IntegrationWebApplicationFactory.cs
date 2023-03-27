using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest;

public class IntegrationWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var client = new IntegrationDbClient();
            services.AddSingleton<IAmazonDynamoDB>(client);
        });
    }
}
