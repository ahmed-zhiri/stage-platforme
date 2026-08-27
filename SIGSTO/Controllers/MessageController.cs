using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGSTO.Data;
using SIGSTO.Models;

namespace SIGSTO.Controllers
{
    public class MessageController : Controller
    {
        private readonly AppDbContext _db;

        public MessageController(AppDbContext db)
        {
            _db = db;
        }

        private int UserId => HttpContext.Session.GetInt32("UserId") ?? 0;
        private string Role => HttpContext.Session.GetString("Role") ?? "";

        public IActionResult Conversation(int etudiantId, int encadrantId)
        {
            if (UserId == 0) return RedirectToAction("Login", "Auth");

            // Verify the user is part of this conversation
            if (Role == "Etudiant" && etudiantId != UserId) return Forbid();
            if (Role == "Encadrant" && encadrantId != UserId) return Forbid();

            // Check assignment exists
            var hasAssignment = _db.Candidatures.Any(c =>
                c.EtudiantId == etudiantId && c.EncadrantId == encadrantId);
            if (!hasAssignment)
            {
                TempData["Error"] = "Pas de stage assigne entre ces utilisateurs.";
                return Role == "Etudiant"
                    ? RedirectToAction("Dashboard", "Etudiant")
                    : RedirectToAction("Dashboard", "Encadrant");
            }

            var messages = _db.Messages
                .Include(m => m.Etudiant)
                .Include(m => m.Encadrant)
                .Where(m => m.EtudiantId == etudiantId && m.EncadrantId == encadrantId)
                .OrderBy(m => m.DateEnvoi)
                .ToList();

            ViewBag.EtudiantId = etudiantId;
            ViewBag.EncadrantId = encadrantId;
            ViewBag.Role = Role;

            var otherUser = Role == "Etudiant"
                ? (Utilisateur?)_db.Encadrants.Find(encadrantId)
                : (Utilisateur?)_db.Etudiants.Find(etudiantId);
            ViewBag.OtherUser = otherUser;

            return View(messages);
        }

        [HttpPost]
        public IActionResult Envoyer(int etudiantId, int encadrantId, string contenu, IFormFile? fichier)
        {
            if (UserId == 0) return RedirectToAction("Login", "Auth");

            var message = new Message
            {
                EtudiantId = etudiantId,
                EncadrantId = encadrantId,
                Contenu = contenu ?? "",
                DateEnvoi = DateTime.Now,
                Expediteur = Role == "Etudiant" ? ExpediteurMessage.Etudiant : ExpediteurMessage.Encadrant
            };

            if (fichier != null && fichier.Length > 0 && fichier.Length <= 5 * 1024 * 1024)
            {
                var dir = Path.Combine("wwwroot", "uploads", "messages");
                Directory.CreateDirectory(dir);
                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{fichier.FileName}";
                var path = Path.Combine(dir, fileName);
                using var stream = new FileStream(path, FileMode.Create);
                fichier.CopyTo(stream);
                message.CheminFichier = path.Replace("wwwroot/", "/").Replace("wwwroot\\", "/");
            }

            _db.Messages.Add(message);
            _db.SaveChanges();

            return RedirectToAction("Conversation", new { etudiantId, encadrantId });
        }
    }
}
