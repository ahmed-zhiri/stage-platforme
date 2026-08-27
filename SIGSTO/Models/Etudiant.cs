using System.ComponentModel.DataAnnotations;

namespace SIGSTO.Models
{
    public class Etudiant : Utilisateur
    {
        [MaxLength(200)]
        public string Etablissement { get; set; } = "";

        [MaxLength(200)]
        public string Filiere { get; set; } = "";

        public DateTime? DateNaissance { get; set; }

        [MaxLength(20)]
        public string Sexe { get; set; } = "";

        public bool Handicap { get; set; }

        public OTP? OTP { get; set; }
    }
}
