using ApplicationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationService();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseAuthorization();

app.UseSession();

app.Run();

public partial class Program { }
