using CartonCaps.Api.Domain.Enums;

namespace CartonCaps.Api.Domain.Entities;

public class Referral
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ReferrerUserId { get; set; }
    public Guid? RefereeUserId { get; set; }
    public ReferralStatus ReferralStatus { get; set; } = ReferralStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public User Referrer { get; set; }
    public User Referee { get; set; }
}
