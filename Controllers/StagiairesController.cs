using GestionStagiaires.Web.DTOs;
using GestionStagiaires.Web.Models.Entities;
using GestionStagiaires.Web.Services;
using GestionStagiaires.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionStagiaires.Web.Controllers;

/// <summary>
/// Contrôleur MVC (back-office) pour la gestion CRUD des stagiaires.
/// Nécessite une authentification.
/// </summary>
[Authorize]
public class StagiairesController : Controller
{
    private readonly IStagiaireService _service;

    public StagiairesController(IStagiaireService service)
    {
        _service = service;
    }

    // GET: /Stagiaires?recherche=...&statut=...&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> Index(string? recherche, StatutStage? statut, int page = 1, int pageSize = 10)
    {
        PagedResult<StagiaireDto> resultat = await _service.GetPagedAsync(recherche, statut, page, pageSize);

        var vm = new StagiaireListViewModel
        {
            Page = resultat,
            Recherche = recherche,
            Statut = statut,
            PageSize = pageSize
        };

        return View(vm);
    }

    // GET: /Stagiaires/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        StagiaireDto? dto = await _service.GetByIdAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    // GET: /Stagiaires/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new StagiaireCreateDto());
    }

    // POST: /Stagiaires/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StagiaireCreateDto dto)
    {
        if (await _service.EmailExistsAsync(dto.Email))
        {
            ModelState.AddModelError(nameof(dto.Email), "Un stagiaire avec cet email existe déjà.");
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        StagiaireDto cree = await _service.CreateAsync(dto);
        TempData["Success"] = $"Le stagiaire « {cree.NomComplet} » a été créé avec succès.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Stagiaires/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        StagiaireUpdateDto? dto = await _service.GetForEditAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    // POST: /Stagiaires/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StagiaireUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest();

        if (await _service.EmailExistsAsync(dto.Email, dto.Id))
        {
            ModelState.AddModelError(nameof(dto.Email), "Un autre stagiaire utilise déjà cet email.");
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        StagiaireDto? maj = await _service.UpdateAsync(dto);
        if (maj is null) return NotFound();

        TempData["Success"] = $"Le stagiaire « {maj.NomComplet} » a été mis à jour.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Stagiaires/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        StagiaireDto? dto = await _service.GetByIdAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    // POST: /Stagiaires/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool supprime = await _service.DeleteAsync(id);
        TempData[supprime ? "Success" : "Error"] = supprime
            ? "Le stagiaire a été supprimé."
            : "Stagiaire introuvable.";
        return RedirectToAction(nameof(Index));
    }
}
