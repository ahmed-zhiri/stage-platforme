using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGSTO.Data;
using SIGSTO.Filters;
using SIGSTO.Models;

namespace SIGSTO.Controllers
{
    [AuthFilter("Etudiant")]
    public class EtudiantController : Controller
    {
        private readonly AppDbContext _db;

        public EtudiantController(AppDbContext db)
        {
            _db = db;
        }

        private int UserId => HttpContext.Session.GetInt32("UserId") ?? 0;

        public IActionResult Dashboard()
        {
            var etudiant = _db.Etudiants.Find(UserId);
            var candidatures = _db.Candidatures
                .Include(c => c.Offre)
                .Include(c => c.Encadrant)
                .Where(c => c.EtudiantId == UserId)
                .OrderByDescending(c => c.DateSoumission)
                .ToList();

            ViewBag.Etudiant = etudiant;
            ViewBag.Candidatures = candidatures;
            return View();
        }

        public IActionResult Profil()
        {
            var etudiant = _db.Etudiants.Find(UserId);
            return View(etudiant);
        }

        [HttpPost]
        public IActionResult Profil(string etablissement, string filiere, DateTime? dateNaissance, string sexe, bool handicap)
        {
            var etudiant = _db.Etudiants.Find(UserId);
            if (etudiant == null) return RedirectToAction("Login", "Auth");

            etudiant.Etablissement = etablissement ?? "";
            etudiant.Filiere = filiere ?? "";
            etudiant.DateNaissance = dateNaissance;
            etudiant.Sexe = sexe ?? "";
            etudiant.Handicap = handicap;
            _db.SaveChanges();

            TempData["Success"] = "Profil mis a jour avec succes.";
            return RedirectToAction("Profil");
        }

        public IActionResult Offres()
        {
            var offres = _db.Offres
                .Where(o => o.Statut == StatutOffre.Ouvert && o.DateLimitePostule >= DateTime.Now)
                .OrderByDescending(o => o.DateLimitePostule)
                .ToList();
            return View(offres);
        }

        public IActionResult DetailOffre(int id)
        {
            var offre = _db.Offres.Find(id);
            if (offre == null) return NotFound();

            var dejaPostule = _db.Candidatures.Any(c => c.EtudiantId == UserId && c.OffreId == id);
            ViewBag.DejaPostule = dejaPostule;
            return View(offre);
        }

        public IActionResult Postuler(int id)
        {
            var offre = _db.Offres.Find(id);
            if (offre == null || offre.Statut != StatutOffre.Ouvert)
                return RedirectToAction("Offres");

            if (_db.Candidatures.Any(c => c.EtudiantId == UserId && c.OffreId == id))
            {
                TempData["Error"] = "Vous avez deja postule a cette offre.";
                return RedirectToAction("Offres");
            }

            ViewBag.Offre = offre;
            return View();
        }

        [HttpPost]
        public IActionResult Postuler(int offreId, IFormFile cv, IFormFile lm, IFormFile lr, IFormFile releves)
        {
            var offre = _db.Offres.Find(offreId);
            if (offre == null || offre.Statut != StatutOffre.Ouvert)
                return RedirectToAction("Offres");

            if (cv == null)
            {
                ViewBag.Error = "Le CV est obligatoire.";
                ViewBag.Offre = offre;
                return View();
            }

            var candidature = new Candidature
            {
                EtudiantId = UserId,
                OffreId = offreId,
                DateSoumission = DateTime.Now,
                StatutCandidature = StatutCandidature.Soumise
            };
            _db.Candidatures.Add(candidature);
            _db.SaveChanges();

            var uploadDir = Path.Combine("wwwroot", "uploads", candidature.Id.ToString());
            Directory.CreateDirectory(uploadDir);

            candidature.CheminCV = SaveFile(cv, uploadDir, "cv.pdf");
            if (lm != null) candidature.CheminLM = SaveFile(lm, uploadDir, "lm.pdf");
            if (lr != null) candidature.CheminLR = SaveFile(lr, uploadDir, "lr.pdf");
            if (releves != null) candidature.CheminReleves = SaveFile(releves, uploadDir, "releves.pdf");

            // CV Scoring
            var scoringService = HttpContext.RequestServices.GetRequiredService<SIGSTO.Services.CVScoringService>();
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadDir, "cv.pdf");
            var texte = scoringService.ExtraireTexte(fullPath);
            candidature.Score = scoringService.CalculerScore(texte, offre.MotsCles);

            _db.SaveChanges();

            TempData["Success"] = "Candidature soumise avec succes.";
            return RedirectToAction("Dashboard");
        }

        public IActionResult DetailCandidature(int id)
        {
            var candidature = _db.Candidatures
                .Include(c => c.Offre)
                .Include(c => c.Encadrant)
                .Include(c => c.Convention)
                .Include(c => c.AccordDeStage)
                .Include(c => c.Attestation)
                .Include(c => c.Evaluation)
                .FirstOrDefault(c => c.Id == id && c.EtudiantId == UserId);

            if (candidature == null) return NotFound();
            return View(candidature);
        }

        private string SaveFile(IFormFile file, string dir, string filename)
        {
            if (file == null || file.Length == 0) return "";
            if (file.Length > 5 * 1024 * 1024) return "";
            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) return "";

            var path = Path.Combine(dir, filename);
            using var stream = new FileStream(path, FileMode.Create);
            file.CopyTo(stream);

            return path.Replace("wwwroot/", "/").Replace("wwwroot\\", "/");
        }
    }
}
