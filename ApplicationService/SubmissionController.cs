using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
    [AllowAnonymous]
    public async Task<IActionResult> Start()
    {
        var userId = new Guid().ToString();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, "Applicant")
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        var submission = new SubmissionEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = SubmissionStatus.InProgress,
            CreatedDate = DateTime.Now
        };

        await _submissionRepo.Add(submission);
        return Ok();
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] SubmissionModel submission)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

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
