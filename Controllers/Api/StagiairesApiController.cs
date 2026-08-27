using GestionStagiaires.Web.DTOs;
using GestionStagiaires.Web.Models.Entities;
using GestionStagiaires.Web.Services;
using GestionStagiaires.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionStagiaires.Web.Controllers.Api;

/// <summary>
/// API REST de gestion des stagiaires, consommée par le front-office
/// (jQuery / AJAX) et par tout client tiers.
/// </summary>
[ApiController]
[Route("api/stagiaires")]
[Authorize]
[Produces("application/json")]
public class StagiairesApiController : ControllerBase
{
    private readonly IStagiaireService _service;

    public StagiairesApiController(IStagiaireService service)
    {
        _service = service;
    }

    /// <summary>Liste paginée avec recherche et filtre par statut.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<StagiaireDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<StagiaireDto>>> GetAll(
        [FromQuery] string? recherche,
        [FromQuery] StatutStage? statut,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        PagedResult<StagiaireDto> resultat =
            await _service.GetPagedAsync(recherche, statut, page, pageSize, cancellationToken);
        return Ok(resultat);
    }

    /// <summary>Détail d'un stagiaire.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StagiaireDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StagiaireDto>> GetById(int id, CancellationToken cancellationToken)
    {
        StagiaireDto? dto = await _service.GetByIdAsync(id, cancellationToken);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Création d'un stagiaire.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(StagiaireDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StagiaireDto>> Create(
        [FromBody] StagiaireCreateDto dto, CancellationToken cancellationToken)
    {
        if (await _service.EmailExistsAsync(dto.Email, null, cancellationToken))
        {
            ModelState.AddModelError(nameof(dto.Email), "Un stagiaire avec cet email existe déjà.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        StagiaireDto cree = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = cree.Id }, cree);
    }

    /// <summary>Mise à jour complète d'un stagiaire.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(StagiaireDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StagiaireDto>> Update(
        int id, [FromBody] StagiaireUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            ModelState.AddModelError(nameof(dto.Id), "L'identifiant de l'URL et du corps ne correspondent pas.");
            return ValidationProblem(ModelState);
        }

        if (await _service.EmailExistsAsync(dto.Email, dto.Id, cancellationToken))
        {
            ModelState.AddModelError(nameof(dto.Email), "Un autre stagiaire utilise déjà cet email.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        StagiaireDto? maj = await _service.UpdateAsync(dto, cancellationToken);
        return maj is null ? NotFound() : Ok(maj);
    }

    /// <summary>Suppression d'un stagiaire.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        bool supprime = await _service.DeleteAsync(id, cancellationToken);
        return supprime ? NoContent() : NotFound();
    }
}
