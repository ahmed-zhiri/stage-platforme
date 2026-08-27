using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGSTO.Models
{
    public class AccordDeStage
    {
        public int Id { get; set; }

        public int CandidatureId { get; set; }

        [ForeignKey("CandidatureId")]
        public Candidature? Candidature { get; set; }

        [MaxLength(300)]
        public string Theme { get; set; } = "";

        [MaxLength(200)]
        public string Periode { get; set; } = "";

        [MaxLength(500)]
        public string CheminFichier { get; set; } = "";

        public DateTime DateAttache { get; set; } = DateTime.Now;
    }
}
