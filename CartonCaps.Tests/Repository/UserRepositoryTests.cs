using CartonCaps.Api.Domain.Entities;
using CartonCaps.Api.Infraestructure;
using CartonCaps.Api.Infraestructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Tests.Repository;

public class UserRepositoryTests
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
    public async Task GetInfo_WhenUserExists_ReturnsUserDto()
    {
        var context = GetInMemoryContext();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            ReferralCode = "TEST2024"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        var result = await repository.GetInfo(userId);

        result.Should().NotBeNull();
        result.Name.Should().Be("Test User");
        result.Email.Should().Be("test@example.com");
        result.ReferralCode.Should().Be("TEST2024");
    }
}
