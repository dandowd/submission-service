namespace ApplicationService;

public interface IUserManager
{
    string GetUserId();
    Task<string> Create();
}
