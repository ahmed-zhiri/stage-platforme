using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGSTO.Models
{
    public class Evaluation
    {
        public int Id { get; set; }

        public int CandidatureId { get; set; }

        [ForeignKey("CandidatureId")]
        public Candidature? Candidature { get; set; }

        public int EncadrantId { get; set; }

        [ForeignKey("EncadrantId")]
        public Encadrant? Encadrant { get; set; }

        public float Note { get; set; }

        public string Appreciation { get; set; } = "";

        public DateTime DateEvaluation { get; set; } = DateTime.Now;
    }
}
