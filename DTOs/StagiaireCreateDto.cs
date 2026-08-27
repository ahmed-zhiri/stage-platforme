using System.ComponentModel.DataAnnotations;
using GestionStagiaires.Web.Models.Entities;

namespace GestionStagiaires.Web.DTOs;

/// <summary>
/// DTO / ViewModel de création d'un stagiaire.
/// Porte les règles de validation appliquées lors de la saisie du formulaire
/// (côté serveur via ModelState et côté client via jQuery Validation).
/// Implémente IValidatableObject pour une validation métier croisée.
/// </summary>
public class StagiaireCreateDto : IValidatableObject
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 80 caractères.")]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 80 caractères.")]
    [Display(Name = "Prénom")]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse email est obligatoire.")]
    [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
    [StringLength(150)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
    [StringLength(20)]
    [Display(Name = "Téléphone")]
    public string? Telephone { get; set; }

    [Required(ErrorMessage = "L'établissement est obligatoire.")]
    [StringLength(150)]
    [Display(Name = "Établissement")]
    public string Etablissement { get; set; } = string.Empty;

    [StringLength(120)]
    [Display(Name = "Spécialité / Filière")]
    public string? Specialite { get; set; }

    [StringLength(250)]
    [Display(Name = "Sujet du stage")]
    public string? Sujet { get; set; }

    [StringLength(120)]
    [Display(Name = "Département d'accueil")]
    public string? Departement { get; set; }

    [Required(ErrorMessage = "La date de début est obligatoire.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date de début")]
    public DateTime DateDebut { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "La date de fin est obligatoire.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date de fin")]
    public DateTime DateFin { get; set; } = DateTime.Today.AddMonths(2);

    [Required]
    [Display(Name = "Statut")]
    public StatutStage Statut { get; set; } = StatutStage.EnAttente;

    /// <summary>
    /// Validation métier : la date de fin doit être postérieure à la date de début.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateFin.Date <= DateDebut.Date)
        {
            yield return new ValidationResult(
                "La date de fin doit être postérieure à la date de début.",
                new[] { nameof(DateFin) });
        }
    }
}
