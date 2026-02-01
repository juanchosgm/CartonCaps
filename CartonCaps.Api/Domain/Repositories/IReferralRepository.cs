using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Entities;
using System.Linq.Expressions;

namespace CartonCaps.Api.Domain.Repositories;

public interface IReferralRepository
{
    Task<bool> ReferralExists(Expression<Func<Referral, bool>> expression);
    Task Create(Referral referral);
    Task<IEnumerable<ReferralDto>> GetReferralsCompleted(Guid userId);
    Task<Referral> GetReferral(Guid referralId);
    Task<int> CountCompletedReferralsByDate(Guid userId, DateTime fromDate);
    Task<IEnumerable<Referral>> GetPendingReferralsOlderThan(DateTime cutoffDate);
    Task SaveChangesAsync();
}
