using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGSTO.Models
{
    public class Candidature
    {
        public int Id { get; set; }

        public int EtudiantId { get; set; }

        [ForeignKey("EtudiantId")]
        public Etudiant? Etudiant { get; set; }

        public int OffreId { get; set; }

        [ForeignKey("OffreId")]
        public Offre? Offre { get; set; }

        public int? EncadrantId { get; set; }

        [ForeignKey("EncadrantId")]
        public Encadrant? Encadrant { get; set; }

        public float Score { get; set; }

        public StatutCandidature StatutCandidature { get; set; } = StatutCandidature.Soumise;

        public DateTime DateSoumission { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string CheminCV { get; set; } = "";

        [MaxLength(500)]
        public string CheminLM { get; set; } = "";

        [MaxLength(500)]
        public string CheminLR { get; set; } = "";

        [MaxLength(500)]
        public string CheminReleves { get; set; } = "";

        public Convention? Convention { get; set; }
        public AccordDeStage? AccordDeStage { get; set; }
        public Attestation? Attestation { get; set; }
        public Evaluation? Evaluation { get; set; }
    }
}
