using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGSTO.Models
{
    public class Convention
    {
        public int Id { get; set; }

        public int CandidatureId { get; set; }

        [ForeignKey("CandidatureId")]
        public Candidature? Candidature { get; set; }

        [MaxLength(500)]
        public string CheminConv { get; set; } = "";

        [MaxLength(500)]
        public string CheminAssurance { get; set; } = "";

        public StatutConvention Statut { get; set; } = StatutConvention.EnAttente;
    }
}
