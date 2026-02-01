using CartonCaps.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Api.Infraestructure;

public class CartonCapsContext : DbContext
{
    public CartonCapsContext(DbContextOptions<CartonCapsContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Referral> Referrals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
