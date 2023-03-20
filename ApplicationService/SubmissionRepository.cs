namespace ApplicationService;

public class SubmissionRepository : IRepository<SubmissionEntity>
{
    public Task Add(SubmissionEntity entity)
    {
        return Task.CompletedTask;
    }

    public Task Delete(SubmissionEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SubmissionEntity>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<SubmissionEntity> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task Update(SubmissionEntity entity)
    {
        return Task.CompletedTask;
    }
}
