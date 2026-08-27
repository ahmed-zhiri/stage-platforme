using Microsoft.EntityFrameworkCore;
using SIGSTO.Models;

namespace SIGSTO.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Encadrant> Encadrants { get; set; }
        public DbSet<GestionnaireDesStages> Gestionnaires { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Offre> Offres { get; set; }
        public DbSet<Candidature> Candidatures { get; set; }
        public DbSet<Convention> Conventions { get; set; }
        public DbSet<AccordDeStage> AccordsDeStage { get; set; }
        public DbSet<Attestation> Attestations { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TPH inheritance for Utilisateur hierarchy
            modelBuilder.Entity<Utilisateur>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Utilisateur>("Utilisateur")
                .HasValue<Etudiant>("Etudiant")
                .HasValue<Encadrant>("Encadrant")
                .HasValue<GestionnaireDesStages>("Gestionnaire");

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // OTP -> Etudiant one-to-one
            modelBuilder.Entity<OTP>()
                .HasOne(o => o.Etudiant)
                .WithOne(e => e.OTP)
                .HasForeignKey<OTP>(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Convention -> Candidature one-to-one
            modelBuilder.Entity<Convention>()
                .HasOne(c => c.Candidature)
                .WithOne(c => c.Convention)
                .HasForeignKey<Convention>(c => c.CandidatureId)
                .OnDelete(DeleteBehavior.Cascade);

            // AccordDeStage -> Candidature one-to-one
            modelBuilder.Entity<AccordDeStage>()
                .HasOne(a => a.Candidature)
                .WithOne(c => c.AccordDeStage)
                .HasForeignKey<AccordDeStage>(a => a.CandidatureId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attestation -> Candidature one-to-one
            modelBuilder.Entity<Attestation>()
                .HasOne(a => a.Candidature)
                .WithOne(c => c.Attestation)
                .HasForeignKey<Attestation>(a => a.CandidatureId)
                .OnDelete(DeleteBehavior.Cascade);

            // Evaluation -> Candidature one-to-one
            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Candidature)
                .WithOne(c => c.Evaluation)
                .HasForeignKey<Evaluation>(e => e.CandidatureId)
                .OnDelete(DeleteBehavior.Cascade);

            // Message relationships - disable cascade to avoid multiple cascade paths
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Etudiant)
                .WithMany()
                .HasForeignKey(m => m.EtudiantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Encadrant)
                .WithMany()
                .HasForeignKey(m => m.EncadrantId)
                .OnDelete(DeleteBehavior.NoAction);

            // Candidature relationships
            modelBuilder.Entity<Candidature>()
                .HasOne(c => c.Etudiant)
                .WithMany()
                .HasForeignKey(c => c.EtudiantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Candidature>()
                .HasOne(c => c.Offre)
                .WithMany(o => o.Candidatures)
                .HasForeignKey(c => c.OffreId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Candidature>()
                .HasOne(c => c.Encadrant)
                .WithMany()
                .HasForeignKey(c => c.EncadrantId)
                .OnDelete(DeleteBehavior.NoAction);

            // Evaluation -> Encadrant
            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Encadrant)
                .WithMany()
                .HasForeignKey(e => e.EncadrantId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
