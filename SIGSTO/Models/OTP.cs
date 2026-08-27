using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGSTO.Models
{
    public class OTP
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Etudiant? Etudiant { get; set; }

        [Required, MaxLength(6)]
        public string OTPCode { get; set; } = "";

        public DateTime Expiration { get; set; }

        public bool Utilise { get; set; }
    }
}
