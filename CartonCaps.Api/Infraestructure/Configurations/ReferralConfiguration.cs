using CartonCaps.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartonCaps.Api.Infraestructure.Configurations;

public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
{
    public void Configure(EntityTypeBuilder<Referral> builder)
    {
        builder.ToTable("Referrals");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).IsRequired();

        builder.Property(r => r.ReferrerUserId)
            .IsRequired();

        builder.Property(r => r.RefereeUserId)
            .IsRequired(false);

        builder.Property(r => r.ReferralStatus)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.CompletedAt)
            .IsRequired(false);

        builder.HasOne(r => r.Referrer)
            .WithMany()
            .HasForeignKey(r => r.ReferrerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Referee)
            .WithMany()
            .HasForeignKey(r => r.RefereeUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.ReferrerUserId);
        builder.HasIndex(r => r.RefereeUserId);
        builder.HasIndex(r => r.ReferralStatus);
        builder.HasIndex(r => r.CreatedAt);
    }
}
