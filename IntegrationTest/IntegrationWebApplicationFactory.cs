using Amazon.DynamoDBv2;
using Amazon.Runtime;
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
            var creds = new BasicAWSCredentials("access_key_id", "secret_key");
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000",
                UseHttp = true,
            };
            var client = new AmazonDynamoDBClient(creds, config);
            services.AddSingleton<IAmazonDynamoDB>(client);
        });
    }
}
