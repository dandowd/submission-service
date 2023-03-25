using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService;

[Route("api/[controller]")]
[ApiController]
public class SubmissionController : ControllerBase
{
    private readonly ILogger<SubmissionController> _logger;
    private readonly IRepository<SubmissionEntity> _submissionRepo;
    private readonly ISessionManager _sessionManager;
    private readonly IPublish _publisher;
    private readonly IMapper _mapper;

    public SubmissionController(
        IRepository<SubmissionEntity> submissionRepo,
        ISessionManager sessionManager,
        IMapper mapper,
        IPublish publisher,
        ILogger<SubmissionController> logger
    )
    {
        _sessionManager = sessionManager;
        _publisher = publisher;
        _mapper = mapper;
        _submissionRepo = submissionRepo;
        _logger = logger;
    }

    [HttpPost("start")]
    [AllowAnonymous]
    public async Task<IActionResult> Start()
    {
        var userId = await _sessionManager.Create();

        var submission = new SubmissionEntity
        {
            Id = userId,
            Status = SubmissionStatus.InProgress,
            CreatedDate = DateTime.Now
        };

        await _submissionRepo.Add(submission);

        return Ok();
    }

    [HttpPatch("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] SubmissionModel submission)
    {
        var submissionId = _sessionManager.GetUserId();

        var submissionEntity = _mapper.Map<SubmissionEntity>(submission);
        submissionEntity.Id = submissionId;

        await _submissionRepo.Update(submissionEntity);

        return Ok();
    }

    [HttpPost("complete")]
    [Authorize]
    public async Task<IActionResult> Complete()
    {
        var submissionId = _sessionManager.GetUserId();

        var submission = await _submissionRepo.GetById(submissionId);
        submission.Status = SubmissionStatus.Completed;
        submission.CompletedDate = DateTime.Now;

        await _submissionRepo.Update(submission);
        _publisher.Publish(submission);

        return Ok();
    }
}
