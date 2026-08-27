using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGSTO.Data;
using SIGSTO.Filters;
using SIGSTO.Models;
using System.Security.Cryptography;
using System.Text;

namespace SIGSTO.Controllers
{
    [AuthFilter("Gestionnaire")]
    public class GestionnaireController : Controller
    {
        private readonly AppDbContext _db;

        public GestionnaireController(AppDbContext db)
        {
            _db = db;
        }

        private int UserId => HttpContext.Session.GetInt32("UserId") ?? 0;

        public IActionResult Dashboard()
        {
            var offres = _db.Offres
                .Where(o => o.GestionnaireId == UserId)
                .OrderByDescending(o => o.Id)
                .ToList();

            var totalCandidatures = _db.Candidatures
                .Count(c => c.Offre != null && c.Offre.GestionnaireId == UserId);

            ViewBag.Offres = offres;
            ViewBag.TotalCandidatures = totalCandidatures;
            return View();
        }

        public IActionResult CreerOffre()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreerOffre(Offre offre)
        {
            offre.GestionnaireId = UserId;
            offre.Statut = StatutOffre.Ouvert;
            _db.Offres.Add(offre);
            _db.SaveChanges();

            TempData["Success"] = "Offre creee avec succes.";
            return RedirectToAction("Dashboard");
        }

        public IActionResult ModifierOffre(int id)
        {
            var offre = _db.Offres.FirstOrDefault(o => o.Id == id && o.GestionnaireId == UserId);
            if (offre == null) return NotFound();
            return View(offre);
        }

        [HttpPost]
        public IActionResult ModifierOffre(int id, Offre model)
        {
            var offre = _db.Offres.FirstOrDefault(o => o.Id == id && o.GestionnaireId == UserId);
            if (offre == null) return NotFound();

            offre.Titre = model.Titre;
            offre.Filiere = model.Filiere;
            offre.Description = model.Description;
            offre.MotsCles = model.MotsCles;
            offre.NbrPlaces = model.NbrPlaces;
            offre.DateDebut = model.DateDebut;
            offre.DateFin = model.DateFin;
            offre.DateLimitePostule = model.DateLimitePostule;
            offre.Type = model.Type;
            offre.Statut = model.Statut;
            _db.SaveChanges();

            TempData["Success"] = "Offre modifiee avec succes.";
            return RedirectToAction("Dashboard");
        }

        public IActionResult Candidatures(int offreId)
        {
            var offre = _db.Offres.FirstOrDefault(o => o.Id == offreId && o.GestionnaireId == UserId);
            if (offre == null) return NotFound();

            var candidatures = _db.Candidatures
                .Include(c => c.Etudiant)
                .Include(c => c.Encadrant)
                .Where(c => c.OffreId == offreId)
                .OrderByDescending(c => c.Score)
                .ToList();

            ViewBag.Offre = offre;
            return View(candidatures);
        }

        [HttpPost]
        public IActionResult ChangerStatut(int candidatureId, StatutCandidature statut)
        {
            var candidature = _db.Candidatures
                .Include(c => c.Offre)
                .FirstOrDefault(c => c.Id == candidatureId && c.Offre!.GestionnaireId == UserId);
            if (candidature == null) return NotFound();

            candidature.StatutCandidature = statut;

            // If accepted, auto-close other candidatures for this student
            if (statut == StatutCandidature.Acceptee)
            {
                var otherCandidatures = _db.Candidatures
                    .Where(c => c.EtudiantId == candidature.EtudiantId
                                && c.Id != candidature.Id
                                && c.StatutCandidature != StatutCandidature.Acceptee
                                && c.StatutCandidature != StatutCandidature.Cloturee)
                    .ToList();
                foreach (var other in otherCandidatures)
                {
                    other.StatutCandidature = StatutCandidature.Refusee;
                }
            }

            _db.SaveChanges();
            return RedirectToAction("Candidatures", new { offreId = candidature.OffreId });
        }

        public IActionResult DetailCandidature(int id)
        {
            var candidature = _db.Candidatures
                .Include(c => c.Etudiant)
                .Include(c => c.Offre)
                .Include(c => c.Encadrant)
                .Include(c => c.Convention)
                .Include(c => c.AccordDeStage)
                .Include(c => c.Attestation)
                .Include(c => c.Evaluation)
                .FirstOrDefault(c => c.Id == id && c.Offre!.GestionnaireId == UserId);

            if (candidature == null) return NotFound();

            ViewBag.Encadrants = _db.Encadrants.ToList();
            return View(candidature);
        }

        [HttpPost]
        public IActionResult AttacherAccord(int candidatureId, string theme, string periode, IFormFile fichier)
        {
            var candidature = _db.Candidatures
                .Include(c => c.Offre)
                .Include(c => c.AccordDeStage)
                .FirstOrDefault(c => c.Id == candidatureId && c.Offre!.GestionnaireId == UserId);
            if (candidature == null) return NotFound();

            var cheminFichier = "";
            if (fichier != null && fichier.Length > 0)
            {
                var dir = Path.Combine("wwwroot", "uploads", candidatureId.ToString());
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, "accord.pdf");
                using var stream = new FileStream(path, FileMode.Create);
                fichier.CopyTo(stream);
                cheminFichier = path.Replace("wwwroot/", "/").Replace("wwwroot\\", "/");
            }

            if (candidature.AccordDeStage == null)
            {
                candidature.AccordDeStage = new AccordDeStage
                {
                    CandidatureId = candidatureId,
                    Theme = theme ?? "",
                    Periode = periode ?? "",
                    CheminFichier = cheminFichier,
                    DateAttache = DateTime.Now
                };
            }
            else
            {
                candidature.AccordDeStage.Theme = theme ?? "";
                candidature.AccordDeStage.Periode = periode ?? "";
                if (!string.IsNullOrEmpty(cheminFichier))
                    candidature.AccordDeStage.CheminFichier = cheminFichier;
            }

            _db.SaveChanges();
            TempData["Success"] = "Accord de stage attache.";
            return RedirectToAction("DetailCandidature", new { id = candidatureId });
        }

        [HttpPost]
        public IActionResult AssignerEncadrant(int candidatureId, int encadrantId)
        {
            var candidature = _db.Candidatures
                .Include(c => c.Offre)
                .Include(c => c.Convention)
                .Include(c => c.AccordDeStage)
                .FirstOrDefault(c => c.Id == candidatureId && c.Offre!.GestionnaireId == UserId);
            if (candidature == null) return NotFound();

            // Check convention and accord exist
            if (candidature.Convention == null || candidature.AccordDeStage == null)
            {
                TempData["Error"] = "La convention et l'accord de stage doivent etre attaches avant d'assigner un encadrant.";
                return RedirectToAction("DetailCandidature", new { id = candidatureId });
            }

            // Check encadrant max 10 active
            var activeCount = _db.Candidatures.Count(c =>
                c.EncadrantId == encadrantId
                && c.StatutCandidature != StatutCandidature.Cloturee
                && c.StatutCandidature != StatutCandidature.Refusee);

            if (activeCount >= 10)
            {
                TempData["Error"] = "Cet encadrant a deja 10 candidatures actives.";
                return RedirectToAction("DetailCandidature", new { id = candidatureId });
            }

            candidature.EncadrantId = encadrantId;
            candidature.StatutCandidature = StatutCandidature.EnCours;
            _db.SaveChanges();

            TempData["Success"] = "Encadrant assigne avec succes.";
            return RedirectToAction("DetailCandidature", new { id = candidatureId });
        }

        [HttpPost]
        public IActionResult GenererAttestation(int candidatureId)
        {
            var candidature = _db.Candidatures
                .Include(c => c.Offre)
                .Include(c => c.Etudiant)
                .Include(c => c.Evaluation)
                .Include(c => c.Attestation)
                .FirstOrDefault(c => c.Id == candidatureId && c.Offre!.GestionnaireId == UserId);
            if (candidature == null) return NotFound();

            if (candidature.Evaluation == null)
            {
                TempData["Error"] = "L'evaluation doit etre soumise avant de generer l'attestation.";
                return RedirectToAction("DetailCandidature", new { id = candidatureId });
            }

            // Placeholder attestation - simple text file
            var dir = Path.Combine("wwwroot", "uploads", candidatureId.ToString());
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, "attestation.txt");

            var content = $@"ATTESTATION DE STAGE
===================

ONEE - Office National de l'Electricite et de l'Eau Potable

Nous attestons que {candidature.Etudiant?.Prenom} {candidature.Etudiant?.Nom}
a effectue un stage au sein de notre organisme.

Offre: {candidature.Offre?.Titre}
Type: {candidature.Offre?.Type}
Periode: {candidature.Offre?.DateDebut:dd/MM/yyyy} - {candidature.Offre?.DateFin:dd/MM/yyyy}

Note d'evaluation: {candidature.Evaluation.Note}/20
Appreciation: {candidature.Evaluation.Appreciation}

Date: {DateTime.Now:dd/MM/yyyy}
";

            System.IO.File.WriteAllText(path, content);

            if (candidature.Attestation == null)
            {
                candidature.Attestation = new Attestation
                {
                    CandidatureId = candidatureId,
                    DateGen = DateTime.Now,
                    CheminFichier = path.Replace("wwwroot/", "/").Replace("wwwroot\\", "/")
                };
            }
            else
            {
                candidature.Attestation.DateGen = DateTime.Now;
                candidature.Attestation.CheminFichier = path.Replace("wwwroot/", "/").Replace("wwwroot\\", "/");
            }

            _db.SaveChanges();
            TempData["Success"] = "Attestation generee.";
            return RedirectToAction("DetailCandidature", new { id = candidatureId });
        }

        public IActionResult CreerEncadrant()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreerEncadrant(string nom, string prenom, string email, string password, string departement)
        {
            if (_db.Utilisateurs.Any(u => u.Email == email))
            {
                ViewBag.Error = "Cet email est deja utilise.";
                return View();
            }

            var encadrant = new Encadrant
            {
                Nom = nom,
                Prenom = prenom,
                Email = email,
                Password = HashPassword(password),
                Role = RoleUtilisateur.Encadrant,
                EmailVerifie = true,
                Departement = departement ?? ""
            };

            _db.Encadrants.Add(encadrant);
            _db.SaveChanges();

            TempData["Success"] = "Encadrant cree avec succes.";
            return RedirectToAction("Dashboard");
        }

        public IActionResult Encadrants()
        {
            var encadrants = _db.Encadrants.ToList();
            return View(encadrants);
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
