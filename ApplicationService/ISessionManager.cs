namespace ApplicationService;

public interface ISessionManager
{
    string GetUserId();
    Task<string> Create();
}
