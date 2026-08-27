using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGSTO.Data;
using SIGSTO.Filters;
using SIGSTO.Models;

namespace SIGSTO.Controllers
{
    [AuthFilter("Encadrant")]
    public class EncadrantController : Controller
    {
        private readonly AppDbContext _db;

        public EncadrantController(AppDbContext db)
        {
            _db = db;
        }

        private int UserId => HttpContext.Session.GetInt32("UserId") ?? 0;

        public IActionResult Dashboard()
        {
            var candidatures = _db.Candidatures
                .Include(c => c.Etudiant)
                .Include(c => c.Offre)
                .Include(c => c.Evaluation)
                .Where(c => c.EncadrantId == UserId)
                .OrderByDescending(c => c.DateSoumission)
                .ToList();

            return View(candidatures);
        }

        public IActionResult DetailCandidature(int id)
        {
            var candidature = _db.Candidatures
                .Include(c => c.Etudiant)
                .Include(c => c.Offre)
                .Include(c => c.Convention)
                .Include(c => c.AccordDeStage)
                .Include(c => c.Evaluation)
                .FirstOrDefault(c => c.Id == id && c.EncadrantId == UserId);

            if (candidature == null) return NotFound();
            return View(candidature);
        }

        public IActionResult Evaluer(int candidatureId)
        {
            var candidature = _db.Candidatures
                .Include(c => c.Etudiant)
                .Include(c => c.Offre)
                .Include(c => c.Evaluation)
                .FirstOrDefault(c => c.Id == candidatureId && c.EncadrantId == UserId);

            if (candidature == null) return NotFound();
            if (candidature.Evaluation != null)
            {
                TempData["Error"] = "L'evaluation a deja ete soumise.";
                return RedirectToAction("DetailCandidature", new { id = candidatureId });
            }

            return View(candidature);
        }

        [HttpPost]
        public IActionResult Evaluer(int candidatureId, float note, string appreciation)
        {
            var candidature = _db.Candidatures
                .Include(c => c.Evaluation)
                .FirstOrDefault(c => c.Id == candidatureId && c.EncadrantId == UserId);

            if (candidature == null) return NotFound();
            if (candidature.Evaluation != null)
            {
                TempData["Error"] = "L'evaluation a deja ete soumise.";
                return RedirectToAction("Dashboard");
            }

            var evaluation = new Evaluation
            {
                CandidatureId = candidatureId,
                EncadrantId = UserId,
                Note = note,
                Appreciation = appreciation ?? "",
                DateEvaluation = DateTime.Now
            };

            _db.Evaluations.Add(evaluation);
            _db.SaveChanges();

            TempData["Success"] = "Evaluation soumise avec succes.";
            return RedirectToAction("Dashboard");
        }
    }
}
