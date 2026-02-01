using CartonCaps.Api.Domain.Configurations;
using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Entities;
using CartonCaps.Api.Domain.Enums;
using CartonCaps.Api.Domain.Functionals;
using CartonCaps.Api.Domain.Interfaces.BL;
using CartonCaps.Api.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CartonCaps.Api.BL;

public class ReferralService : IReferralService
{
    private readonly IUserRepository userRepository;
    private readonly IReferralRepository referralRepository;
    private readonly IValidator<ReferralRegisterDto> validator;
    private readonly IOptions<FrontendConfiguration> frontendSettings;
    private readonly IOptions<ReferralLimitsConfiguration> referralLimitsSettings;

    public ReferralService(
        IUserRepository userRepository,
        IReferralRepository referralRepository,
        IValidator<ReferralRegisterDto> validator,
        IOptions<FrontendConfiguration> frontendSettings,
        IOptions<ReferralLimitsConfiguration> referralLimitsSettings)
    {
        this.userRepository = userRepository;
        this.referralRepository = referralRepository;
        this.validator = validator;
        this.frontendSettings = frontendSettings;
        this.referralLimitsSettings = referralLimitsSettings;
    }

    public async Task<Result<InvitationLinkDto>> GenerateInvitation(Guid userId)
    {
        var user = await userRepository.GetInfo(userId);
        if (user == null) return Result.Fail<InvitationLinkDto>("User not found");
        var referral = new Referral
        {
            ReferrerUserId = userId,
            CreatedAt = DateTime.UtcNow,
        };
        await referralRepository.Create(referral);
        await referralRepository.SaveChangesAsync();
        var baseUrl = frontendSettings.Value.BaseUrl;
        return Result.Ok(new InvitationLinkDto($"{baseUrl}/signup?ref={referral.Id}&code={user.ReferralCode}"));
    }

    public async Task<IEnumerable<ReferralDto>> GetReferralsByUser(Guid userId) =>
        await referralRepository.GetReferralsCompleted(userId);

    public async Task<Result> RegisterReferral(ReferralRegisterDto model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid) return Result.Fail(string.Join('|', validation.Errors.Select(e => e.ErrorMessage)));

        if (model.ReferralId.HasValue)
        {
            var referral = await referralRepository.GetReferral(model.ReferralId.Value);

            if (referral.ReferralStatus == ReferralStatus.Expired)
                return Result.Fail("This referral link has expired");

            var dailyLimitReached = await CheckDailyReferralLimit(referral.ReferrerUserId);
            if (!dailyLimitReached) return Result.Fail("Daily referral limit reached");
        }

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            ReferralCode = await GenerateReferralCode(model.Name),
        };
        await userRepository.Create(user);
        if (model.ReferralId.HasValue)
        {
            var referral = await referralRepository.GetReferral(model.ReferralId.Value);
            referral.RefereeUserId = user.Id;
            referral.CompletedAt = DateTime.UtcNow;
            referral.ReferralStatus = ReferralStatus.Completed;
        }
        await referralRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<bool> CheckDailyReferralLimit(Guid userId)
    {
        var today = DateTime.UtcNow.Date;
        var referralsToday = await referralRepository.CountCompletedReferralsByDate(userId, today);
        var maxDailyReferrals = referralLimitsSettings.Value.MaxReferralsPerDay;
        return referralsToday < maxDailyReferrals;
    }

    private async Task<string> GenerateReferralCode(string userName)
    {
        var firstName = userName.Split(' ')[0];
        var baseCode = firstName.ToUpper();
        var year = DateTime.UtcNow.Year;
        var referralCode = $"{baseCode}{year}";
        var codeExists = await userRepository.UserExists(u => u.ReferralCode == referralCode);
        var counter = 1;
        while (codeExists)
        {
            counter++;
            referralCode = $"{baseCode}{year}_{counter}";
            codeExists = await userRepository.UserExists(u => u.ReferralCode == referralCode);
        }
        return referralCode;
    }
}
