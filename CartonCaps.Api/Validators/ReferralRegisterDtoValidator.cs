using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Repositories;
using FluentValidation;

namespace CartonCaps.Api.Validators;

public class ReferralRegisterDtoValidator : AbstractValidator<ReferralRegisterDto>
{
    private readonly IUserRepository userRepository;
    private readonly IReferralRepository referralRepository;

    public ReferralRegisterDtoValidator(IUserRepository userRepository, IReferralRepository referralRepository)
    {
        this.userRepository = userRepository;
        this.referralRepository = referralRepository;

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (x, token) => !await userRepository.UserExists(u => u.Email.Equals(x)))
            .WithMessage("Email already exists");

        RuleFor(x => x.ReferralCode)
            .MustAsync(async (x, token) => await userRepository.UserExists(u => u.ReferralCode.Equals(x)))
            .When(x => !string.IsNullOrWhiteSpace(x.ReferralCode))
            .WithMessage("Referral Code is not valid");

        RuleFor(x => x.ReferralId)
            .MustAsync(async (x, token) => await referralRepository.ReferralExists(r => r.Id == x))
            .When(x => x.ReferralId.HasValue)
            .WithMessage("Referral Id is not valid");

        RuleFor(x => x)
            .MustAsync(IsNotSelfReferral)
            .WithMessage("You cannot use your own referral link")
            .When(x => x.ReferralId.HasValue);
    }

    private async Task<bool> IsNotSelfReferral(ReferralRegisterDto dto, CancellationToken token)
    {
        if (!dto.ReferralId.HasValue) return true;

        var referral = await referralRepository.GetReferral(dto.ReferralId.Value);
        if (referral == null) return true;

        var referrer = await userRepository.GetInfo(referral.ReferrerUserId);
        if (referrer == null) return true;

        return !referrer.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase);
    }
}
