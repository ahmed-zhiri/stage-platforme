using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGSTO.Models
{
    public class Offre
    {
        public int Id { get; set; }

        public int GestionnaireId { get; set; }

        [ForeignKey("GestionnaireId")]
        public GestionnaireDesStages? Gestionnaire { get; set; }

        [Required, MaxLength(300)]
        public string Titre { get; set; } = "";

        [MaxLength(200)]
        public string Filiere { get; set; } = "";

        public string Description { get; set; } = "";

        public string MotsCles { get; set; } = "";

        public int NbrPlaces { get; set; }

        public DateTime DateDebut { get; set; }

        public DateTime DateFin { get; set; }

        public DateTime DateLimitePostule { get; set; }

        public TypeStage Type { get; set; }

        public StatutOffre Statut { get; set; } = StatutOffre.Ouvert;

        public List<Candidature> Candidatures { get; set; } = new();
    }
}
