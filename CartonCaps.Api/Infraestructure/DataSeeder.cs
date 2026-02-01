using CartonCaps.Api.Domain.Entities;

namespace CartonCaps.Api.Infraestructure;

public static class DataSeeder
{
    public static void SeedData(CartonCapsContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var users = new List<User>
        {
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "María García",
                Email = "maria.garcia@example.com",
                ReferralCode = "MARIA2024"
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Laura Martínez",
                Email = "laura.martinez@example.com",
                ReferralCode = "LAURA2024"
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Carlos López",
                Email = "carlos.lopez@example.com",
                ReferralCode = "CARLOS2024"
            },
            new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Ana Rodríguez",
                Email = "ana.rodriguez@example.com",
                ReferralCode = "ANA2024"
            },
            new User
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Pedro Sánchez",
                Email = "pedro.sanchez@example.com",
                ReferralCode = "PEDRO2024"
            }
        };

        context.Users.AddRange(users);

        context.SaveChanges();
    }
}
