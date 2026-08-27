using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGSTO.Data;
using SIGSTO.Models;
using SIGSTO.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SIGSTO.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly EmailService _emailService;

        public AuthController(AppDbContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToDashboard();
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _db.Utilisateurs.FirstOrDefault(u => u.Email == email);
            if (user == null || user.Password != HashPassword(password))
            {
                ViewBag.Error = "Email ou mot de passe incorrect.";
                return View();
            }

            if (!user.EmailVerifie)
            {
                ViewBag.Error = "Votre email n'est pas encore verifie.";
                return View();
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Error = "Vous n'avez pas encore defini de mot de passe.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.Prenom + " " + user.Nom);

            return RedirectToDashboard();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string nom, string prenom, string email)
        {
            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@etu\.uae\.ac\.ma$"))
            {
                ViewBag.Error = "Veuillez utiliser votre email institutionnel (@etu.uae.ac.ma).";
                return View();
            }

            if (_db.Utilisateurs.Any(u => u.Email == email))
            {
                ViewBag.Error = "Cet email est deja utilise.";
                return View();
            }

            var etudiant = new Etudiant
            {
                Nom = nom,
                Prenom = prenom,
                Email = email,
                Password = "",
                Role = RoleUtilisateur.Etudiant,
                EmailVerifie = false
            };

            _db.Etudiants.Add(etudiant);
            _db.SaveChanges();

            var otpCode = new Random().Next(100000, 999999).ToString();
            var otp = new OTP
            {
                UserId = etudiant.Id,
                OTPCode = otpCode,
                Expiration = DateTime.Now.AddMinutes(10),
                Utilise = false
            };
            _db.OTPs.Add(otp);
            _db.SaveChanges();

            try
            {
                _emailService.EnvoyerOTP(email, otpCode);
                TempData["Success"] = "Un code OTP a ete envoye a votre email.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erreur lors de l'envoi de l'email: " + ex.Message;
            }

            TempData["UserId"] = etudiant.Id;
            return RedirectToAction("VerifyOTP");
        }

        public IActionResult VerifyOTP()
        {
            ViewBag.UserId = TempData["UserId"] ?? HttpContext.Session.GetInt32("PendingUserId");
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOTP(int userId, string otpCode)
        {
            var otp = _db.OTPs
                .Where(o => o.UserId == userId && o.OTPCode == otpCode && !o.Utilise)
                .OrderByDescending(o => o.Id)
                .FirstOrDefault();

            if (otp == null)
            {
                ViewBag.Error = "Code OTP invalide.";
                ViewBag.UserId = userId;
                return View();
            }

            if (otp.Expiration < DateTime.Now)
            {
                ViewBag.Error = "Le code OTP a expire. Veuillez en demander un nouveau.";
                ViewBag.UserId = userId;
                return View();
            }

            otp.Utilise = true;
            var etudiant = _db.Etudiants.Find(userId);
            if (etudiant != null)
            {
                etudiant.EmailVerifie = true;
            }
            _db.SaveChanges();

            TempData["Success"] = "Email verifie avec succes. Veuillez creer votre mot de passe.";
            TempData["UserId"] = userId;
            return RedirectToAction("SetPassword");
        }

        [HttpPost]
        public IActionResult RenvoyerOTP(int userId)
        {
            var etudiant = _db.Etudiants.Find(userId);
            if (etudiant == null || etudiant.EmailVerifie)
                return RedirectToAction("Login");

            var otpCode = new Random().Next(100000, 999999).ToString();
            var otp = new OTP
            {
                UserId = etudiant.Id,
                OTPCode = otpCode,
                Expiration = DateTime.Now.AddMinutes(10),
                Utilise = false
            };
            _db.OTPs.Add(otp);
            _db.SaveChanges();

            try
            {
                _emailService.EnvoyerOTP(etudiant.Email, otpCode);
                TempData["Success"] = "Un nouveau code OTP a ete envoye.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erreur lors de l'envoi: " + ex.Message;
            }

            TempData["UserId"] = etudiant.Id;
            return RedirectToAction("VerifyOTP");
        }

        public IActionResult SetPassword()
        {
            ViewBag.UserId = TempData["UserId"];
            if (ViewBag.UserId == null)
                return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public IActionResult SetPassword(int userId, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                ViewBag.Error = "Le mot de passe doit contenir au moins 6 caracteres.";
                ViewBag.UserId = userId;
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Les mots de passe ne correspondent pas.";
                ViewBag.UserId = userId;
                return View();
            }

            var etudiant = _db.Etudiants.Find(userId);
            if (etudiant == null)
                return RedirectToAction("Login");

            etudiant.Password = HashPassword(password);
            _db.SaveChanges();

            TempData["Success"] = "Mot de passe cree avec succes. Vous pouvez maintenant vous connecter.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private IActionResult RedirectToDashboard()
        {
            var role = HttpContext.Session.GetString("Role");
            return role switch
            {
                "Etudiant" => RedirectToAction("Dashboard", "Etudiant"),
                "Gestionnaire" => RedirectToAction("Dashboard", "Gestionnaire"),
                "Encadrant" => RedirectToAction("Dashboard", "Encadrant"),
                _ => RedirectToAction("Login")
            };
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
