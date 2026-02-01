using CartonCaps.Api.BL;
using CartonCaps.Api.Domain.Configurations;
using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Entities;
using CartonCaps.Api.Domain.Enums;
using CartonCaps.Api.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Moq;

namespace CartonCaps.Tests.BL;

public class ReferralServiceTests
{
    [Fact]
    public async Task GenerateInvitation_UserDoesNotExist_ReturnsError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockReferralRepo = new Mock<IReferralRepository>();
        var mockValidator = new Mock<IValidator<ReferralRegisterDto>>();

        mockUserRepo.Setup(r => r.GetInfo(userId))
            .ReturnsAsync((UserDto)null);

        var mockFrontendSettings = new Mock<IOptions<FrontendConfiguration>>();
        mockFrontendSettings.Setup(x => x.Value).Returns(new FrontendConfiguration { BaseUrl = "http://localhost:4200" });

        var mockReferralLimits = new Mock<IOptions<ReferralLimitsConfiguration>>();
        mockReferralLimits.Setup(x => x.Value).Returns(new ReferralLimitsConfiguration { MaxReferralsPerDay = 10, LinkExpirationDays = 7 });

        var service = new ReferralService(mockUserRepo.Object, mockReferralRepo.Object, mockValidator.Object, mockFrontendSettings.Object, mockReferralLimits.Object);

        // Act
        var result = await service.GenerateInvitation(userId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StringErrors.Should().Contain("User not found");
    }

    [Fact]
    public async Task GenerateInvitation_ValidUser_CreatesInvitationLink()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockReferralRepo = new Mock<IReferralRepository>();
        var mockValidator = new Mock<IValidator<ReferralRegisterDto>>();

        var userDto = new UserDto("Juan Perez", "juan@test.com", "JUAN2026");

        mockUserRepo.Setup(r => r.GetInfo(userId))
            .ReturnsAsync(userDto);

        var mockFrontendSettings = new Mock<IOptions<FrontendConfiguration>>();
        mockFrontendSettings.Setup(x => x.Value).Returns(new FrontendConfiguration { BaseUrl = "http://localhost:4200" });

        var mockReferralLimits = new Mock<IOptions<ReferralLimitsConfiguration>>();
        mockReferralLimits.Setup(x => x.Value).Returns(new ReferralLimitsConfiguration { MaxReferralsPerDay = 10, LinkExpirationDays = 7 });

        var service = new ReferralService(mockUserRepo.Object, mockReferralRepo.Object, mockValidator.Object, mockFrontendSettings.Object, mockReferralLimits.Object);

        // Act
        var result = await service.GenerateInvitation(userId);

        // Assert
        var value = result.Value;
        result.IsSuccess.Should().BeTrue();
        value.Link.Should().StartWith("http://localhost:4200/signup?ref=");
        value.Link.Should().Contain("code=JUAN2026");
        mockReferralRepo.Verify(r => r.Create(It.IsAny<Referral>()), Times.Once);
        mockReferralRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterReferral_InvalidData_ReturnsValidationErrors()
    {
        // Arrange
        var mockUserRepo = new Mock<IUserRepository>();
        var mockReferralRepo = new Mock<IReferralRepository>();
        var mockValidator = new Mock<IValidator<ReferralRegisterDto>>();

        var model = new ReferralRegisterDto("", "invalid-email", "", null);

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "El nombre es requerido"),
            new ValidationFailure("Email", "Email inválido")
        };

        mockValidator.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var mockFrontendSettings = new Mock<IOptions<FrontendConfiguration>>();
        mockFrontendSettings.Setup(x => x.Value).Returns(new FrontendConfiguration { BaseUrl = "http://localhost:4200" });

        var mockReferralLimits = new Mock<IOptions<ReferralLimitsConfiguration>>();
        mockReferralLimits.Setup(x => x.Value).Returns(new ReferralLimitsConfiguration { MaxReferralsPerDay = 10, LinkExpirationDays = 7 });

        var service = new ReferralService(mockUserRepo.Object, mockReferralRepo.Object, mockValidator.Object, mockFrontendSettings.Object, mockReferralLimits.Object);

        // Act
        var result = await service.RegisterReferral(model);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StringErrors.Should().Contain("nombre");
        result.StringErrors.Should().Contain("Email");
    }

    [Fact]
    public async Task RegisterReferral_WithReferralId_CompletesReferral()
    {
        // Arrange
        var mockUserRepo = new Mock<IUserRepository>();
        var mockReferralRepo = new Mock<IReferralRepository>();
        var mockValidator = new Mock<IValidator<ReferralRegisterDto>>();

        var referralId = Guid.NewGuid();
        var model = new ReferralRegisterDto("Maria Lopez", "maria@test.com", "", referralId);

        var existingReferral = new Referral
        {
            Id = referralId,
            ReferrerUserId = Guid.NewGuid(),
            ReferralStatus = ReferralStatus.Pending
        };

        mockValidator.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new ValidationResult());

        mockUserRepo.Setup(r => r.UserExists(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(false);

        mockReferralRepo.Setup(r => r.GetReferral(referralId))
            .ReturnsAsync(existingReferral);

        mockReferralRepo.Setup(r => r.CountCompletedReferralsByDate(It.IsAny<Guid>(), It.IsAny<DateTime>()))
            .ReturnsAsync(0);

        var mockFrontendSettings = new Mock<IOptions<FrontendConfiguration>>();
        mockFrontendSettings.Setup(x => x.Value).Returns(new FrontendConfiguration { BaseUrl = "http://localhost:4200" });

        var mockReferralLimits = new Mock<IOptions<ReferralLimitsConfiguration>>();
        mockReferralLimits.Setup(x => x.Value).Returns(new ReferralLimitsConfiguration { MaxReferralsPerDay = 10, LinkExpirationDays = 7 });

        var service = new ReferralService(mockUserRepo.Object, mockReferralRepo.Object, mockValidator.Object, mockFrontendSettings.Object, mockReferralLimits.Object);

        // Act
        var result = await service.RegisterReferral(model);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingReferral.ReferralStatus.Should().Be(ReferralStatus.Completed);
        existingReferral.CompletedAt.Should().NotBeNull();
        mockUserRepo.Verify(r => r.Create(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task GetReferralsByUser_ReturnsCompletedReferrals()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockReferralRepo = new Mock<IReferralRepository>();
        var mockValidator = new Mock<IValidator<ReferralRegisterDto>>();

        var expectedReferrals = new List<ReferralDto>
        {
            new ReferralDto("Carlos", ReferralStatus.Completed),
            new ReferralDto("Ana", ReferralStatus.Completed)
        };

        mockReferralRepo.Setup(r => r.GetReferralsCompleted(userId))
            .ReturnsAsync(expectedReferrals);

        var mockFrontendSettings = new Mock<IOptions<FrontendConfiguration>>();
        mockFrontendSettings.Setup(x => x.Value).Returns(new FrontendConfiguration { BaseUrl = "http://localhost:4200" });

        var mockReferralLimits = new Mock<IOptions<ReferralLimitsConfiguration>>();
        mockReferralLimits.Setup(x => x.Value).Returns(new ReferralLimitsConfiguration { MaxReferralsPerDay = 10, LinkExpirationDays = 7 });

        var service = new ReferralService(mockUserRepo.Object, mockReferralRepo.Object, mockValidator.Object, mockFrontendSettings.Object, mockReferralLimits.Object);

        // Act
        var result = await service.GetReferralsByUser(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Name == "Carlos");
    }
}
