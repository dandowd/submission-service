namespace IntegrationTest;

public class Test : IClassFixture<IntegrationWebApplicationFactory<Program>>
{
    private readonly IntegrationWebApplicationFactory<Program> _factory;

    public Test(IntegrationWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public HttpResponseMessage SetupSession(HttpClient client)
    {
        var response = client
            .SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/submission/start"))
            .Result.EnsureSuccessStatusCode();

        return response;
    }

    [Fact]
    public void Post_StartShouldCreateNewSubmissionUsingSessionAuth()
    {
        var client = _factory.CreateClient();

        var response = client
            .SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/submission/start"))
            .Result.EnsureSuccessStatusCode();

        Assert.NotNull(response.Headers.GetValues("Set-Cookie").FirstOrDefault());
    }

    [Fact]
    void Post_UpdateShouldUpdateCreatedSubmission()
    {
        var client = _factory.CreateClient();
        var sessionResponse = SetupSession(client);
        var sessionCookie = sessionResponse.Headers.GetValues("Set-Cookie").FirstOrDefault();

        Assert.NotNull(sessionCookie);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/submission/update");
        request.Headers.Add("Cookie", sessionCookie);

        var response = client.SendAsync(request);

        response.Result.EnsureSuccessStatusCode();
    }
}
