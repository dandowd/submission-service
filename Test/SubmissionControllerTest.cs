using ApplicationService;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Moq;

namespace Test;

public class SubmissionControllerTest
{
    Mock<ILogger<SubmissionController>> _mockLogger = new Mock<ILogger<SubmissionController>>();
    Mock<IRepository<SubmissionEntity>> _mockRepo = new Mock<IRepository<SubmissionEntity>>();
    Mock<IPublish> _mockPublish = new Mock<IPublish>();
    IMapper _mapper = new MapperConfiguration(
        cfg => cfg.AddProfile(new MapperProfile())
    ).CreateMapper();
    Mock<ISessionManager> _mockSessionManager = new Mock<ISessionManager>();

    private SubmissionController BuildController()
    {
        _mockRepo.Setup(repo => repo.Add(It.IsAny<SubmissionEntity>())).Verifiable();
        _mockSessionManager.Setup(manager => manager.GetUserId()).Returns("1234");
        _mockRepo
            .Setup(repo => repo.GetById(It.IsAny<string>()))
            .ReturnsAsync(new SubmissionEntity { Id = "1234" });

        return new SubmissionController(
            submissionRepo: _mockRepo.Object,
            sessionManager: _mockSessionManager.Object,
            mapper: _mapper,
            logger: _mockLogger.Object,
            publisher: _mockPublish.Object
        );
    }

    private void ResetMocks()
    {
        _mockLogger.Reset();
        _mockRepo.Reset();
    }

    [Fact]
    public async Task Start_Should_Return_Ok()
    {
        ResetMocks();

        var controller = BuildController();
        var result = await controller.Start();

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Update_Should_Return_Ok()
    {
        ResetMocks();
        var controller = BuildController();

        var submission = new SubmissionModel { FirstName = "John", LastName = "Doe", };
        var result = await controller.Update(submission);

        _mockRepo.Verify(
            repo =>
                repo.Update(
                    It.Is<SubmissionEntity>(obj => obj.FirstName == "John" && obj.LastName == "Doe")
                ),
            Times.Once
        );
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Complete_Should_Return_Ok()
    {
        ResetMocks();
        var controller = BuildController();

        var result = await controller.Complete();

        Assert.IsType<OkResult>(result);
    }
}
