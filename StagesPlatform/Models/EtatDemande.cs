namespace StagesPlatform.Models
{
    /// <summary>
    /// Etats possibles d'une demande de stage dans le workflow de validation.
    /// </summary>
    public enum EtatDemande
    {
        Brouillon = 0,
        Soumise = 1,
        ValideeEncadrant = 2,
        ValideeRH = 3,
        Rejetee = 4,
        EnCours = 5,
        Terminee = 6,
        Cloturee = 7
    }
}
