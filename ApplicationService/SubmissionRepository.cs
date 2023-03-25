using Amazon.DynamoDBv2.DataModel;

namespace ApplicationService;

public class SubmissionRepository : IRepository<SubmissionEntity>
{
    private readonly IDynamoDBContext _dynamoDbClient;

    public SubmissionRepository(IDynamoDBContext dynamoDbClient)
    {
        _dynamoDbClient = dynamoDbClient;
    }

    public Task Add(SubmissionEntity entity)
    {
        return _dynamoDbClient.SaveAsync(entity);
    }

    public Task<SubmissionEntity> GetById(string id)
    {
        return _dynamoDbClient.LoadAsync<SubmissionEntity>(id);
    }

    public Task Update(SubmissionEntity entity)
    {
        return _dynamoDbClient.SaveAsync(entity);
    }
}
