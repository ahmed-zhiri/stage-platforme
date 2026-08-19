using Microsoft.EntityFrameworkCore;
using StagesPlatform.Models;

namespace StagesPlatform.Data
{
    /// <summary>
    /// Contexte Entity Framework Core de l'application.
    /// Approche Code First : les tables sont generees a partir des classes.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DemandeStage> DemandesStage { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Donnees initiales pour la demonstration.
            modelBuilder.Entity<DemandeStage>().HasData(
                new DemandeStage
                {
                    Id = 1,
                    Sujet = "Developpement d'une plateforme de gestion des stages",
                    Description = "Conception et developpement d'une application ASP.NET MVC pour dematerialiser la gestion des stages.",
                    NomStagiaire = "Ahmed Zhiri",
                    EmailStagiaire = "ahmed.zhiri@centrale-casablanca.ma",
                    Entreprise = "ONEE - Direction des Systemes d'Information",
                    Encadrant = "Mr. Encadrant DSI",
                    DateDebut = new DateTime(2026, 7, 1),
                    DateFin = new DateTime(2026, 8, 31),
                    TypeStage = TypeStage.Decouverte,
                    Etat = EtatDemande.EnCours,
                    DateCreation = new DateTime(2026, 6, 15)
                },
                new DemandeStage
                {
                    Id = 2,
                    Sujet = "Analyse de donnees hydrogeologiques",
                    Description = "Traitement statistique et modelisation ML sur donnees SNMR.",
                    NomStagiaire = "Etudiant Test",
                    EmailStagiaire = "test@centrale-casablanca.ma",
                    Entreprise = "Laboratoire de Recherche",
                    Encadrant = "Dr. Belhboub",
                    DateDebut = new DateTime(2026, 9, 1),
                    DateFin = new DateTime(2027, 2, 28),
                    TypeStage = TypeStage.FinEtudes,
                    Etat = EtatDemande.Soumise,
                    DateCreation = new DateTime(2026, 8, 1)
                }
            );
        }
    }
}
