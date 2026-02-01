using CartonCaps.Api.Domain.Entities;
using CartonCaps.Api.Domain.Enums;
using CartonCaps.Api.Infraestructure;
using CartonCaps.Api.Infraestructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Tests.Repository;

public class ReferralRepositoryTests
{
    private CartonCapsContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CartonCapsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new CartonCapsContext(options);
        return context;
    }

    [Fact]
    public async Task GetReferral_ExistingId_ReturnsReferral()
    {
        // Arrange
        var context = GetInMemoryContext();
        var referralId = Guid.NewGuid();
        var referral = new Referral
        {
            Id = referralId,
            ReferrerUserId = Guid.NewGuid(),
            ReferralStatus = ReferralStatus.Pending
        };
        context.Referrals.Add(referral);
        await context.SaveChangesAsync();

        var repository = new ReferralRepository(context);

        // Act
        var result = await repository.GetReferral(referralId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(referralId);
    }

    [Fact]
    public async Task GetReferralsCompleted_UserWithCompletedReferrals_ReturnsOnlyCompleted()
    {
        // Arrange
        var context = GetInMemoryContext();
        var userId = Guid.NewGuid();
        var referee = new User
        {
            Id = Guid.NewGuid(),
            Name = "Luis Martinez",
            Email = "luis@test.com",
            ReferralCode = "LUIS2026"
        };

        context.Users.Add(referee);

        var completedReferral = new Referral
        {
            ReferrerUserId = userId,
            RefereeUserId = referee.Id,
            ReferralStatus = ReferralStatus.Completed
        };

        var pendingReferral = new Referral
        {
            ReferrerUserId = userId,
            ReferralStatus = ReferralStatus.Pending
        };

        context.Referrals.AddRange(completedReferral, pendingReferral);
        await context.SaveChangesAsync();

        var repository = new ReferralRepository(context);

        // Act
        var result = await repository.GetReferralsCompleted(userId);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Luis Martinez");
    }

    [Fact]
    public async Task Create_NewReferral_AddsToContext()
    {
        // Arrange
        var context = GetInMemoryContext();
        var repository = new ReferralRepository(context);
        var referral = new Referral
        {
            ReferrerUserId = Guid.NewGuid(),
            ReferralStatus = ReferralStatus.Pending
        };

        // Act
        await repository.Create(referral);
        await repository.SaveChangesAsync();

        // Assert
        context.Referrals.Should().HaveCount(1);
        context.Referrals.First().ReferralStatus.Should().Be(ReferralStatus.Pending);
    }
}
