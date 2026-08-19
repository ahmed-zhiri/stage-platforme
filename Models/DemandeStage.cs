using System.ComponentModel.DataAnnotations;

namespace StagesPlatform.Models
{
    /// <summary>
    /// Entite representant une demande de stage.
    /// Illustre les concepts de POO : encapsulation, validation, enumerations.
    /// </summary>
    public class DemandeStage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le sujet est obligatoire.")]
        [StringLength(200, MinimumLength = 5,
            ErrorMessage = "Le sujet doit contenir entre 5 et 200 caracteres.")]
        [Display(Name = "Sujet du stage")]
        public string Sujet { get; set; } = string.Empty;

        [Required(ErrorMessage = "La description est obligatoire.")]
        [StringLength(1000)]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom du stagiaire est obligatoire.")]
        [StringLength(100)]
        [Display(Name = "Nom du stagiaire")]
        public string NomStagiaire { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        [Display(Name = "Email du stagiaire")]
        public string EmailStagiaire { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'entreprise est obligatoire.")]
        [StringLength(150)]
        [Display(Name = "Entreprise d'accueil")]
        public string Entreprise { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'encadrant est obligatoire.")]
        [StringLength(100)]
        [Display(Name = "Encadrant")]
        public string Encadrant { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date de debut")]
        public DateTime DateDebut { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date de fin")]
        public DateTime DateFin { get; set; } = DateTime.Today.AddMonths(2);

        [Required]
        [Display(Name = "Type de stage")]
        public TypeStage TypeStage { get; set; } = TypeStage.Decouverte;

        [Display(Name = "Etat")]
        public EtatDemande Etat { get; set; } = EtatDemande.Brouillon;

        [Display(Name = "Date de soumission")]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        /// <summary>
        /// Calcule la duree du stage en jours (propriete calculee, non persistee).
        /// </summary>
        public int DureeEnJours => (DateFin - DateDebut).Days;

        /// <summary>
        /// Methode metier illustrant la POO : transition d'etat controlee.
        /// </summary>
        public void Soumettre()
        {
            if (Etat == EtatDemande.Brouillon)
                Etat = EtatDemande.Soumise;
        }
    }
}
