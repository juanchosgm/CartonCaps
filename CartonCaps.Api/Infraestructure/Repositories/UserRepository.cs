using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Entities;
using CartonCaps.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CartonCaps.Api.Infraestructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CartonCapsContext context;

    public UserRepository(CartonCapsContext context)
    {
        this.context = context;
    }

    public async Task<Guid> Create(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user.Id;
    }

    public async Task<bool> UserExists(Expression<Func<User, bool>> expression) => await context.Users.AnyAsync(expression);

    public async Task<UserDto> GetInfo(Guid userId)
    {
        return await context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserDto(u.Name, u.Email, u.ReferralCode))
            .FirstOrDefaultAsync();
    }
}
