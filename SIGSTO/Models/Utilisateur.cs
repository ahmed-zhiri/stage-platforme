using System.ComponentModel.DataAnnotations;

namespace SIGSTO.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nom { get; set; } = "";

        [Required, MaxLength(100)]
        public string Prenom { get; set; } = "";

        [Required, MaxLength(200)]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        public bool EmailVerifie { get; set; }

        public RoleUtilisateur Role { get; set; }
    }
}
