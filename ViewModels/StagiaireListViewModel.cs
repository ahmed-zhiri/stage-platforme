using GestionStagiaires.Web.DTOs;
using GestionStagiaires.Web.Models.Entities;

namespace GestionStagiaires.Web.ViewModels;

/// <summary>
/// ViewModel de la page de liste des stagiaires : combine les résultats
/// paginés avec les critères de recherche/filtre courants afin de les
/// réafficher dans le formulaire et de construire les liens de pagination.
/// </summary>
public class StagiaireListViewModel
{
    public PagedResult<StagiaireDto> Page { get; set; } = new();

    /// <summary>Terme de recherche (nom, prénom, email, établissement).</summary>
    public string? Recherche { get; set; }

    /// <summary>Filtre optionnel par statut.</summary>
    public StatutStage? Statut { get; set; }

    public int PageSize { get; set; } = 10;
}
