using Microsoft.AspNetCore.Authentication.Cookies;

namespace ApplicationService;

public static class ServicesConfig
{
    public static void AddApplicationService(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddAutoMapper(typeof(MapperProfile).Assembly);
        services.AddScoped<IRepository<SubmissionEntity>, SubmissionRepository>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IPublish, Publisher>();
        services.AddHttpContextAccessor();

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
