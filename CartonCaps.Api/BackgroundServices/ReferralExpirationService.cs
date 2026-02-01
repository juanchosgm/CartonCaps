using CartonCaps.Api.Domain.Configurations;
using CartonCaps.Api.Domain.Enums;
using CartonCaps.Api.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace CartonCaps.Api.BackgroundServices;

public class ReferralExpirationService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IOptions<ReferralLimitsConfiguration> referralLimitsSettings;
    private readonly ILogger<ReferralExpirationService> logger;

    public ReferralExpirationService(
        IServiceProvider serviceProvider,
        IOptions<ReferralLimitsConfiguration> referralLimitsSettings,
        ILogger<ReferralExpirationService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.referralLimitsSettings = referralLimitsSettings;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExpirePendingReferrals();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error expiring pending referrals");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ExpirePendingReferrals()
    {
        using var scope = serviceProvider.CreateScope();
        var referralRepository = scope.ServiceProvider.GetRequiredService<IReferralRepository>();

        var expirationDays = referralLimitsSettings.Value.LinkExpirationDays;
        var cutoffDate = DateTime.UtcNow.AddDays(-expirationDays);

        var expiredReferrals = await referralRepository.GetPendingReferralsOlderThan(cutoffDate);

        foreach (var referral in expiredReferrals)
        {
            referral.ReferralStatus = ReferralStatus.Expired;
        }

        if (expiredReferrals.Any())
        {
            await referralRepository.SaveChangesAsync();
            logger.LogInformation("Expired {Count} pending referrals", expiredReferrals.Count());
        }
    }
}
