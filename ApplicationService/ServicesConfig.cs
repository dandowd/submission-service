using Microsoft.AspNetCore.Authentication.Cookies;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace ApplicationService;

public static class ServicesConfig
{
    public static void AddApplicationService(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddAutoMapper(typeof(MapperProfile).Assembly);
        services.AddScoped<IRepository<SubmissionEntity>, SubmissionRepository>();
        services.AddScoped<ISessionManager, SessionManager>();
        services.AddScoped<IPublish, Publisher>();
        services.AddHttpContextAccessor();

        // Add DynamoDB context
        services.AddSingleton<IAmazonDynamoDB>(provider =>
        {
            return new AmazonDynamoDBClient();
        });
        services.AddScoped<IDynamoDBContext, DynamoDBContext>();

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromDays(15);
            options.Cookie.IsEssential = true;
            options.Cookie.HttpOnly = true;
        });

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie();
    }
}
