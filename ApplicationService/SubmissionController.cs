using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace ApplicationService;

[Route("api/[controller]")]
[ApiController]
public class SubmissionController : ControllerBase
{
    private readonly ILogger<SubmissionController> _logger;
    private readonly IRepository<SubmissionEntity> _submissionRepo;
    private readonly IPublish _publisher;
    private readonly IMapper _mapper;

    public SubmissionController(
        IRepository<SubmissionEntity> submissionRepo,
        IMapper mapper,
        IPublish publisher,
        ILogger<SubmissionController> logger
    )
    {
        _publisher = publisher;
        _mapper = mapper;
        _submissionRepo = submissionRepo;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start()
    {
        var submission = new SubmissionEntity
        {
            Id = Guid.NewGuid(),
            Status = SubmissionStatus.InProgress,
            CreatedDate = DateTime.Now
        };

        await _submissionRepo.Add(submission);

        return Ok();
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] SubmissionModel submission)
    {
        var submissionEntity = _mapper.Map<SubmissionEntity>(submission);
        await _submissionRepo.Update(submissionEntity);

        return Ok();
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Complete()
    {
        var submission = await _submissionRepo.GetById(
            new Guid("1c5b1b1a-1b1a-1b1a-1b1a-1b1a1b1a1b1a")
        );
        submission.Status = SubmissionStatus.Completed;
        submission.CompletedDate = DateTime.Now;

        await _submissionRepo.Update(submission);
        _publisher.Publish(submission);

        return Ok();
    }
}
