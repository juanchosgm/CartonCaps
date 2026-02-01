using CartonCaps.Api.Domain.Dtos;
using CartonCaps.Api.Domain.Interfaces.BL;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReferralController : ControllerBase
{
    private readonly Guid userLogged = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly IReferralService referralService;

    public ReferralController(IReferralService referralService)
    {
        this.referralService = referralService;
    }

    [HttpGet("my-referrals")]
    public async Task<IActionResult> GetMyReferrals()
    {
        var referrals = await referralService.GetReferralsByUser(userLogged);
        return Ok(referrals);
    }

    [HttpPost("invitation-link")]
    public async Task<IActionResult> GetInvitationLink()
    {
        var link = await referralService.GenerateInvitation(userLogged);
        if (link)
        {
            return Ok(link.Value);
        }
        return BadRequest(link.StringErrors);
    }

    [HttpPost("referral-registration")]
    public async Task<IActionResult> RegisterReferral([FromBody] ReferralRegisterDto model)
    {
        var result = await referralService.RegisterReferral(model);
        if (result) return Ok();
        return BadRequest(result.StringErrors);
    }
}
