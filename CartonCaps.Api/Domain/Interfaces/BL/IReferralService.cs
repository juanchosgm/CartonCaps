using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Functionals;

namespace CartonCaps.Api.Domain.Interfaces.BL;

public interface IReferralService
{
    Task<IEnumerable<ReferralDto>> GetReferralsByUser(Guid userId);
    Task<Result<InvitationLinkDto>> GenerateInvitation(Guid userId);
    Task<Result> RegisterReferral(ReferralRegisterDto model);
}
