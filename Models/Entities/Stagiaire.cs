using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionStagiaires.Web.Models.Entities;

/// <summary>
/// Entité du domaine représentant un stagiaire.
/// Cette classe est mappée vers la table "Stagiaires" via l'approche Code First.
/// </summary>
public class Stagiaire
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Prenom { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Telephone { get; set; }

    /// <summary>Établissement / université d'origine.</summary>
    [Required]
    [StringLength(150)]
    public string Etablissement { get; set; } = string.Empty;

    /// <summary>Filière ou spécialité.</summary>
    [StringLength(120)]
    public string? Specialite { get; set; }

    /// <summary>Sujet / thème du stage.</summary>
    [StringLength(250)]
    public string? Sujet { get; set; }

    /// <summary>Département d'accueil au sein de l'organisme.</summary>
    [StringLength(120)]
    public string? Departement { get; set; }

    [DataType(DataType.Date)]
    public DateTime DateDebut { get; set; }

    [DataType(DataType.Date)]
    public DateTime DateFin { get; set; }

    public StatutStage Statut { get; set; } = StatutStage.EnAttente;

    // Métadonnées d'audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Nom complet calculé (non mappé en base).</summary>
    [NotMapped]
    public string NomComplet => $"{Prenom} {Nom}";
}
