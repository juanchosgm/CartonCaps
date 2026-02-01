using CartonCaps.Api.Controllers;
using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Enums;
using CartonCaps.Api.Domain.Functionals;
using CartonCaps.Api.Domain.Interfaces.BL;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartonCaps.Tests.API;

public class ReferralControllerTests
{
    [Fact]
    public async Task GetMyReferrals_ReturnsOkWithReferrals()
    {
        // Arrange
        var mockService = new Mock<IReferralService>();
        var referrals = new List<ReferralDto>
        {
            new ReferralDto("Pedro", ReferralStatus.Completed)
        };

        mockService.Setup(s => s.GetReferralsByUser(It.IsAny<Guid>()))
            .ReturnsAsync(referrals);

        var controller = new ReferralController(mockService.Object);

        // Act
        var result = await controller.GetMyReferrals();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(referrals);
    }

    [Fact]
    public async Task GetInvitationLink_ServiceSucceeds_ReturnsOkWithLink()
    {
        // Arrange
        var mockService = new Mock<IReferralService>();
        var invitationLink = new InvitationLinkDto("https://cartoncaps.com/signup?ref=123&code=TEST2026");

        mockService.Setup(s => s.GenerateInvitation(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Ok(invitationLink));

        var controller = new ReferralController(mockService.Object);

        // Act
        var result = await controller.GetInvitationLink();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(invitationLink);
    }

    [Fact]
    public async Task GetInvitationLink_ServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IReferralService>();

        mockService.Setup(s => s.GenerateInvitation(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Fail<InvitationLinkDto>("User not found"));

        var controller = new ReferralController(mockService.Object);

        // Act
        var result = await controller.GetInvitationLink();

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("User not found");
    }

    [Fact]
    public async Task RegisterReferral_ValidModel_ReturnsOk()
    {
        // Arrange
        var mockService = new Mock<IReferralService>();
        var model = new ReferralRegisterDto("Sofia Ramirez", "sofia@test.com", "", null);

        mockService.Setup(s => s.RegisterReferral(model))
            .ReturnsAsync(Result.Ok());

        var controller = new ReferralController(mockService.Object);

        // Act
        var result = await controller.RegisterReferral(model);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task RegisterReferral_ServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IReferralService>();
        var model = new ReferralRegisterDto("", "invalid", "", null);

        mockService.Setup(s => s.RegisterReferral(model))
            .ReturnsAsync(Result.Fail("Validation errors"));

        var controller = new ReferralController(mockService.Object);

        // Act
        var result = await controller.RegisterReferral(model);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Validation errors");
    }
}
