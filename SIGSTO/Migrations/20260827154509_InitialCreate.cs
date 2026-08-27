using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGSTO.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    EmailVerifie = table.Column<bool>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Departement = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Etablissement = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Filiere = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DateNaissance = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Sexe = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Handicap = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EtudiantId = table.Column<int>(type: "INTEGER", nullable: false),
                    EncadrantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Contenu = table.Column<string>(type: "TEXT", nullable: false),
                    CheminFichier = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DateEnvoi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Expediteur = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Utilisateurs_EncadrantId",
                        column: x => x.EncadrantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Utilisateurs_EtudiantId",
                        column: x => x.EtudiantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Offres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GestionnaireId = table.Column<int>(type: "INTEGER", nullable: false),
                    Titre = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Filiere = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    MotsCles = table.Column<string>(type: "TEXT", nullable: false),
                    NbrPlaces = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateLimitePostule = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offres_Utilisateurs_GestionnaireId",
                        column: x => x.GestionnaireId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OTPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    OTPCode = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    Expiration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Utilise = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OTPs_Utilisateurs_UserId",
                        column: x => x.UserId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EtudiantId = table.Column<int>(type: "INTEGER", nullable: false),
                    OffreId = table.Column<int>(type: "INTEGER", nullable: false),
                    EncadrantId = table.Column<int>(type: "INTEGER", nullable: true),
                    Score = table.Column<float>(type: "REAL", nullable: false),
                    StatutCandidature = table.Column<int>(type: "INTEGER", nullable: false),
                    DateSoumission = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CheminCV = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CheminLM = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CheminLR = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CheminReleves = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatures_Offres_OffreId",
                        column: x => x.OffreId,
                        principalTable: "Offres",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Candidatures_Utilisateurs_EncadrantId",
                        column: x => x.EncadrantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Candidatures_Utilisateurs_EtudiantId",
                        column: x => x.EtudiantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccordsDeStage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidatureId = table.Column<int>(type: "INTEGER", nullable: false),
                    Theme = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Periode = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CheminFichier = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DateAttache = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccordsDeStage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccordsDeStage_Candidatures_CandidatureId",
                        column: x => x.CandidatureId,
                        principalTable: "Candidatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attestations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidatureId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateGen = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CheminFichier = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attestations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attestations_Candidatures_CandidatureId",
                        column: x => x.CandidatureId,
                        principalTable: "Candidatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidatureId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheminConv = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CheminAssurance = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conventions_Candidatures_CandidatureId",
                        column: x => x.CandidatureId,
                        principalTable: "Candidatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidatureId = table.Column<int>(type: "INTEGER", nullable: false),
                    EncadrantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<float>(type: "REAL", nullable: false),
                    Appreciation = table.Column<string>(type: "TEXT", nullable: false),
                    DateEvaluation = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluations_Candidatures_CandidatureId",
                        column: x => x.CandidatureId,
                        principalTable: "Candidatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluations_Utilisateurs_EncadrantId",
                        column: x => x.EncadrantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccordsDeStage_CandidatureId",
                table: "AccordsDeStage",
                column: "CandidatureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attestations_CandidatureId",
                table: "Attestations",
                column: "CandidatureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_EncadrantId",
                table: "Candidatures",
                column: "EncadrantId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_EtudiantId",
                table: "Candidatures",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_OffreId",
                table: "Candidatures",
                column: "OffreId");

            migrationBuilder.CreateIndex(
                name: "IX_Conventions_CandidatureId",
                table: "Conventions",
                column: "CandidatureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_CandidatureId",
                table: "Evaluations",
                column: "CandidatureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_EncadrantId",
                table: "Evaluations",
                column: "EncadrantId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_EncadrantId",
                table: "Messages",
                column: "EncadrantId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_EtudiantId",
                table: "Messages",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_Offres_GestionnaireId",
                table: "Offres",
                column: "GestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_OTPs_UserId",
                table: "OTPs",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Email",
                table: "Utilisateurs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccordsDeStage");

            migrationBuilder.DropTable(
                name: "Attestations");

            migrationBuilder.DropTable(
                name: "Conventions");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "OTPs");

            migrationBuilder.DropTable(
                name: "Candidatures");

            migrationBuilder.DropTable(
                name: "Offres");

            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
