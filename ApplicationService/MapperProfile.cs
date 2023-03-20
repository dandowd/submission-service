using AutoMapper;

namespace ApplicationService;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<SubmissionModel, SubmissionEntity>();
        CreateMap<SubmissionEntity, SubmissionModel>();
    }
}
