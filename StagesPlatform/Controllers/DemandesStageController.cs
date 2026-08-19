using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StagesPlatform.Data;
using StagesPlatform.Models;

namespace StagesPlatform.Controllers
{
    /// <summary>
    /// Controleur CRUD pour la gestion des demandes de stage.
    /// Illustre : injection de dependances, actions HTTP, model binding,
    /// validation, requetes LINQ to Entities.
    /// </summary>
    public class DemandesStageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DemandesStageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DemandesStage
        public async Task<IActionResult> Index(string? searchString, EtatDemande? filtreEtat)
        {
            var demandes = _context.DemandesStage.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                demandes = demandes.Where(d =>
                    d.Sujet.Contains(searchString) ||
                    d.NomStagiaire.Contains(searchString) ||
                    d.Entreprise.Contains(searchString));
            }

            if (filtreEtat.HasValue)
            {
                demandes = demandes.Where(d => d.Etat == filtreEtat.Value);
            }

            ViewBag.SearchString = searchString;
            ViewBag.FiltreEtat = filtreEtat;

            return View(await demandes.OrderByDescending(d => d.DateCreation).ToListAsync());
        }

        // GET: DemandesStage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var demande = await _context.DemandesStage.FirstOrDefaultAsync(m => m.Id == id);
            if (demande == null) return NotFound();

            return View(demande);
        }

        // GET: DemandesStage/Create
        public IActionResult Create() => View();

        // POST: DemandesStage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Sujet,Description,NomStagiaire,EmailStagiaire,Entreprise,Encadrant,DateDebut,DateFin,TypeStage")]
            DemandeStage demande)
        {
            if (demande.DateFin <= demande.DateDebut)
            {
                ModelState.AddModelError("DateFin", "La date de fin doit etre posterieure a la date de debut.");
            }

            if (ModelState.IsValid)
            {
                demande.Etat = EtatDemande.Brouillon;
                demande.DateCreation = DateTime.Now;
                _context.Add(demande);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Demande creee avec succes.";
                return RedirectToAction(nameof(Index));
            }
            return View(demande);
        }

        // GET: DemandesStage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var demande = await _context.DemandesStage.FindAsync(id);
            if (demande == null) return NotFound();
            return View(demande);
        }

        // POST: DemandesStage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Sujet,Description,NomStagiaire,EmailStagiaire,Entreprise,Encadrant,DateDebut,DateFin,TypeStage,Etat,DateCreation")]
            DemandeStage demande)
        {
            if (id != demande.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(demande);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Demande mise a jour avec succes.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DemandeExists(demande.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(demande);
        }

        // GET: DemandesStage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var demande = await _context.DemandesStage.FirstOrDefaultAsync(m => m.Id == id);
            if (demande == null) return NotFound();

            return View(demande);
        }

        // POST: DemandesStage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var demande = await _context.DemandesStage.FindAsync(id);
            if (demande != null)
            {
                _context.DemandesStage.Remove(demande);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Demande supprimee.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: DemandesStage/ChangerEtat/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangerEtat(int id, EtatDemande nouvelEtat)
        {
            var demande = await _context.DemandesStage.FindAsync(id);
            if (demande == null) return NotFound();

            demande.Etat = nouvelEtat;
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Etat mis a jour : {nouvelEtat}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private bool DemandeExists(int id) =>
            _context.DemandesStage.Any(e => e.Id == id);
    }
}
