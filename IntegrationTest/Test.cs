using System.Net;
using System.Net.Http.Headers;

namespace IntegrationTest;

public class Test
    : IClassFixture<IntegrationWebApplicationFactory<Program>>,
        IClassFixture<DbContainer>
{
    private readonly IntegrationWebApplicationFactory<Program> _factory;
    private readonly DbContainer _dbConnection;

    public Test(IntegrationWebApplicationFactory<Program> factory, DbContainer dbConnection)
    {
        _factory = factory;
        _dbConnection = dbConnection;
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
    void Patch_UpdateShouldUpdateCreatedSubmission()
    {
        var client = _factory.CreateClient();
        var sessionResponse = SetupSession(client);
        var sessionCookie = sessionResponse.Headers.GetValues("Set-Cookie").FirstOrDefault();

        Assert.NotNull(sessionCookie);

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/submission/update");
        request.Headers.Add("Cookie", sessionCookie);
        request.Content = new StringContent(
            "{\"firstname\":\"test\"}",
            new MediaTypeHeaderValue("application/json")
        );

        var response = client.SendAsync(request);

        response.Result.EnsureSuccessStatusCode();
    }

    [Fact]
    void Patch_UpdateShouldFailWithInvalidCookie()
    {
        var client = _factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/submission/update");
        request.Headers.Add("Cookie", "session=invalid");
        request.Content = new StringContent(
            "{\"firstname\":\"test\"}",
            new MediaTypeHeaderValue("application/json")
        );

        var response = client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode);
    }
}
