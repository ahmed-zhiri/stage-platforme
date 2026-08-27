using GestionStagiaires.Web.Models.Entities;
using GestionStagiaires.Web.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Web.Data;

/// <summary>
/// Contexte de base de données Entity Framework Core.
/// Hérite d'IdentityDbContext pour intégrer les tables d'authentification
/// ASP.NET Core Identity en plus des tables métier.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Stagiaire> Stagiaires => Set<Stagiaire>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Stagiaire>(entity =>
        {
            entity.ToTable("Stagiaires");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Nom).IsRequired().HasMaxLength(80);
            entity.Property(s => s.Prenom).IsRequired().HasMaxLength(80);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(150);
            entity.Property(s => s.Telephone).HasMaxLength(20);
            entity.Property(s => s.Etablissement).IsRequired().HasMaxLength(150);
            entity.Property(s => s.Specialite).HasMaxLength(120);
            entity.Property(s => s.Sujet).HasMaxLength(250);
            entity.Property(s => s.Departement).HasMaxLength(120);
            entity.Property(s => s.Statut).HasConversion<int>();

            // Un email de stagiaire est unique.
            entity.HasIndex(s => s.Email).IsUnique();
        });
    }
}
