namespace IntegrationTest;

public class Test : IClassFixture<IntegrationWebApplicationFactory<Program>>
{
    private readonly IntegrationWebApplicationFactory<Program> _factory;

    public Test(IntegrationWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private void Setup()
    {
        var client = _factory.CreateClient();
    }
}
