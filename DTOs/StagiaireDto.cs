using GestionStagiaires.Web.Models.Entities;

namespace GestionStagiaires.Web.DTOs;

/// <summary>
/// DTO de lecture exposé par l'API et utilisé pour l'affichage.
/// Découple l'entité de domaine de la couche de présentation / réseau.
/// </summary>
public class StagiaireDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string Etablissement { get; set; } = string.Empty;
    public string? Specialite { get; set; }
    public string? Sujet { get; set; }
    public string? Departement { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public StatutStage Statut { get; set; }
    public string StatutLibelle { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
