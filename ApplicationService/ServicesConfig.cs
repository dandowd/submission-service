namespace ApplicationService;

public static class ServicesConfig
{
    public static void AddApplicationService(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddAutoMapper(typeof(MapperProfile).Assembly);
        services.AddScoped<IRepository<SubmissionEntity>, SubmissionRepository>();
        services.AddScoped<IPublish, Publisher>();

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.IsEssential = true;
            options.Cookie.HttpOnly = true;
        });
        services.AddAuthentication();
    }
}
