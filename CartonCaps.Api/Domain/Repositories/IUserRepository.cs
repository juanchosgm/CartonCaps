using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Entities;
using System.Linq.Expressions;

namespace CartonCaps.Api.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> UserExists(Expression<Func<User, bool>> expression);
    Task<Guid> Create(User user);
    Task<UserDto> GetInfo(Guid userId);
}
