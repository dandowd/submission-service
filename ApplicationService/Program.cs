using ApplicationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationService();

var app = builder.Build();
app.Run();

public partial class Program { }
