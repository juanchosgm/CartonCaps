using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Entities;
using CartonCaps.Api.Domain.Enums;
using CartonCaps.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CartonCaps.Api.Infraestructure.Repositories;

public class ReferralRepository : IReferralRepository
{
    private readonly CartonCapsContext context;

    public ReferralRepository(CartonCapsContext context)
    {
        this.context = context;
    }

    public async Task Create(Referral referral) => await context.Referrals.AddAsync(referral);

    public async Task<Referral> GetReferral(Guid referralId) => 
        await context.Referrals.FirstOrDefaultAsync(r => r.Id == referralId);

    public async Task<IEnumerable<ReferralDto>> GetReferralsCompleted(Guid userId)
    {
        var referrals = await context.Referrals
            .Where(r => r.ReferrerUserId == userId && r.ReferralStatus == ReferralStatus.Completed)
            .Select(r => new ReferralDto(r.Referee.Name, r.ReferralStatus))
            .ToListAsync();
        return referrals;
    }

    public async Task<bool> ReferralExists(Expression<Func<Referral, bool>> expression) =>
        await context.Referrals.AnyAsync(expression);

    public async Task<int> CountCompletedReferralsByDate(Guid userId, DateTime fromDate) =>
        await context.Referrals.CountAsync(r => r.ReferrerUserId == userId
            && r.ReferralStatus == ReferralStatus.Completed
            && r.CompletedAt >= fromDate);

    public async Task<IEnumerable<Referral>> GetPendingReferralsOlderThan(DateTime cutoffDate) =>
        await context.Referrals
            .Where(r => r.ReferralStatus == ReferralStatus.Pending && r.CreatedAt < cutoffDate)
            .ToListAsync();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
