using System.ComponentModel.DataAnnotations;

namespace GestionStagiaires.Web.Models.Entities;

/// <summary>
/// Statut du stage d'un stagiaire.
/// </summary>
public enum StatutStage
{
    [Display(Name = "En attente")]
    EnAttente = 0,

    [Display(Name = "En cours")]
    EnCours = 1,

    [Display(Name = "Terminé")]
    Termine = 2,

    [Display(Name = "Annulé")]
    Annule = 3
}
