using GestionStagiaires.Web.DTOs;
using GestionStagiaires.Web.Models.Entities;
using GestionStagiaires.Web.ViewModels;

namespace GestionStagiaires.Web.Services;

/// <summary>
/// Contrat de la couche métier de gestion des stagiaires.
/// L'abstraction permet l'injection de dépendances et facilite les tests.
/// </summary>
public interface IStagiaireService
{
    /// <summary>Retourne une page de stagiaires, avec recherche et filtre par statut.</summary>
    Task<PagedResult<StagiaireDto>> GetPagedAsync(
        string? recherche,
        StatutStage? statut,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<StagiaireDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Version destinée au pré-remplissage du formulaire d'édition.</summary>
    Task<StagiaireUpdateDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);

    Task<StagiaireDto> CreateAsync(StagiaireCreateDto dto, CancellationToken cancellationToken = default);

    Task<StagiaireDto?> UpdateAsync(StagiaireUpdateDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
}
