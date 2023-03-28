using System.Net;
using System.Net.Http.Headers;
using ApplicationService;
using Newtonsoft.Json;

namespace IntegrationTest;

public class Test
    : IClassFixture<IntegrationWebApplicationFactory<Program>>,
        IClassFixture<DbContainer>
{
    private readonly IntegrationWebApplicationFactory<Program> _factory;
    private readonly DbContainer _dbContainer;

    public Test(IntegrationWebApplicationFactory<Program> factory, DbContainer dbContainer)
    {
        _factory = factory;
        _dbContainer = dbContainer;
    }

    public HttpResponseMessage SetupSession(HttpClient client)
    {
        var response = client
            .SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/submission/"))
            .Result.EnsureSuccessStatusCode();

        return response;
    }

    public Func<HttpMethod, string, string, HttpRequestMessage> RequestBuilder(HttpClient client)
    {
        // Setup session cookie
        var response = client
            .SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/submission/"))
            .Result.EnsureSuccessStatusCode();

        var sessionCookie = response.Headers.GetValues("Set-Cookie").FirstOrDefault();

        return (HttpMethod method, string endpoint, string jsonContent) =>
        {
            var request = new HttpRequestMessage(method, endpoint);
            request.Headers.Add("Cookie", sessionCookie);
            request.Content = new StringContent(
                jsonContent,
                new MediaTypeHeaderValue("application/json")
            );

            return request;
        };
    }

    [Fact]
    public void Post_StartShouldCreateNewSubmissionUsingSessionAuth()
    {
        var client = _factory.CreateClient();
        var builder = RequestBuilder(client);

        var response = builder(HttpMethod.Post, "/api/submission/", String.Empty);

        Assert.NotNull(response.Headers.GetValues("Cookie").FirstOrDefault());
    }

    [Fact]
    async Task Patch_UpdateShouldUpdateCreatedSubmission()
    {
        var client = _factory.CreateClient();
        var builder = RequestBuilder(client);

        var request = builder(HttpMethod.Patch, "/api/submission/", "{\"firstname\":\"test\"}");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    async Task Patch_ShouldUpdateWithoutRemovingExistingFields()
    {
        var client = _factory.CreateClient();
        var builder = RequestBuilder(client);

        var firstRequest = builder(
            HttpMethod.Patch,
            "/api/submission/",
            "{\"firstname\":\"first\"}"
        );

        await client.SendAsync(firstRequest);

        var secondRequest = builder(
            HttpMethod.Patch,
            "/api/submission/",
            "{\"lastname\":\"last\"}"
        );

        var response = await client.SendAsync(secondRequest);

        var get = builder(HttpMethod.Get, "/api/submission/", String.Empty);
        var getResponse = await client.SendAsync(get);

        var submission = JsonConvert.DeserializeObject<SubmissionModel>(
            await getResponse.Content.ReadAsStringAsync()
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("first", submission?.FirstName);
        Assert.Equal("last", submission?.LastName);
    }

    [Fact]
    void Patch_UpdateShouldFailWithInvalidCookie()
    {
        var client = _factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/submission/");
        request.Headers.Add("Cookie", "session=invalid");
        request.Content = new StringContent(
            "{\"firstname\":\"test\"}",
            new MediaTypeHeaderValue("application/json")
        );

        var response = client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode);
    }
}
