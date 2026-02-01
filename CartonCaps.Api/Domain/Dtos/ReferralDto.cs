using CartonCaps.Api.Domain.Enums;

namespace CartonCaps.Api.Domain.Dtos;

public record ReferralDto(string Name, ReferralStatus Status);
