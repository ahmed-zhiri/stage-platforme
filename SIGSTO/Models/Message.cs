using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGSTO.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int EtudiantId { get; set; }

        [ForeignKey("EtudiantId")]
        public Etudiant? Etudiant { get; set; }

        public int EncadrantId { get; set; }

        [ForeignKey("EncadrantId")]
        public Encadrant? Encadrant { get; set; }

        [Required]
        public string Contenu { get; set; } = "";

        [MaxLength(500)]
        public string? CheminFichier { get; set; }

        public DateTime DateEnvoi { get; set; } = DateTime.Now;

        public ExpediteurMessage Expediteur { get; set; }
    }
}
