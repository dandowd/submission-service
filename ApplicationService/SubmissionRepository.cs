﻿namespace ApplicationService;

public class SubmissionRepository : IRepository<SubmissionEntity>
{
    public Task Add(SubmissionEntity entity)
    {
        return Task.CompletedTask;
    }

    public Task<SubmissionEntity> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public Task Update(SubmissionEntity entity)
    {
        return Task.CompletedTask;
    }
}
