namespace CartonCaps.Api.Domain.Dtos;

public record ReferralRegisterDto(string Name, string Email, string ReferralCode, Guid? ReferralId);
