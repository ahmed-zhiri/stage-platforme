using GestionStagiaires.Web.Models.Entities;
using GestionStagiaires.Web.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Web.Data;

/// <summary>
/// Initialise la base : applique les migrations, crée le rôle et le compte
/// administrateur, et insère quelques stagiaires de démonstration.
/// </summary>
public static class SeedData
{
    public const string AdminRole = "Administrateur";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        IServiceProvider sp = scope.ServiceProvider;

        ApplicationDbContext context = sp.GetRequiredService<ApplicationDbContext>();

        // Applique automatiquement les migrations Code First au démarrage.
        await context.Database.MigrateAsync();

        RoleManager<IdentityRole> roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<ApplicationUser> userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        IConfiguration config = sp.GetRequiredService<IConfiguration>();

        // 1) Rôle administrateur
        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        // 2) Compte administrateur par défaut (paramétrable via appsettings)
        string adminEmail = config["SeedAdmin:Email"] ?? "admin@stagiaires.local";
        string adminPassword = config["SeedAdmin:Password"] ?? "Admin@123";

        ApplicationUser? admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                NomComplet = "Administrateur"
            };

            IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, AdminRole);
            }
        }

        // 3) Données de démonstration
        if (!await context.Stagiaires.AnyAsync())
        {
            context.Stagiaires.AddRange(
                new Stagiaire
                {
                    Nom = "El Amrani", Prenom = "Yasmine", Email = "yasmine.elamrani@example.com",
                    Telephone = "0612345678", Etablissement = "ENSA Agadir",
                    Specialite = "Génie Informatique", Sujet = "Application web de gestion RH",
                    Departement = "Système d'information",
                    DateDebut = new DateTime(2026, 2, 2), DateFin = new DateTime(2026, 5, 30),
                    Statut = StatutStage.EnCours, CreatedAt = DateTime.UtcNow
                },
                new Stagiaire
                {
                    Nom = "Benali", Prenom = "Karim", Email = "karim.benali@example.com",
                    Telephone = "0655667788", Etablissement = "FST Marrakech",
                    Specialite = "Réseaux & Télécoms", Sujet = "Supervision d'infrastructure IIS",
                    Departement = "Infrastructure",
                    DateDebut = new DateTime(2026, 3, 1), DateFin = new DateTime(2026, 6, 15),
                    Statut = StatutStage.EnAttente, CreatedAt = DateTime.UtcNow
                },
                new Stagiaire
                {
                    Nom = "Tazi", Prenom = "Salma", Email = "salma.tazi@example.com",
                    Telephone = "0699887766", Etablissement = "EMSI Casablanca",
                    Specialite = "Développement .NET", Sujet = "API REST ASP.NET Core",
                    Departement = "Études & Développement",
                    DateDebut = new DateTime(2025, 9, 1), DateFin = new DateTime(2025, 12, 31),
                    Statut = StatutStage.Termine, CreatedAt = DateTime.UtcNow
                });

            await context.SaveChangesAsync();
        }
    }
}
