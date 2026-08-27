using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGSTO.Models
{
    public class Attestation
    {
        public int Id { get; set; }

        public int CandidatureId { get; set; }

        [ForeignKey("CandidatureId")]
        public Candidature? Candidature { get; set; }

        public DateTime DateGen { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string CheminFichier { get; set; } = "";
    }
}
