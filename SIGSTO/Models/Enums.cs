namespace SIGSTO.Models
{
    public enum RoleUtilisateur
    {
        Etudiant,
        Gestionnaire,
        Encadrant
    }

    public enum TypeStage
    {
        OBS,
        PFA,
        PFE
    }

    public enum StatutOffre
    {
        Ouvert,
        Ferme,
        Pourvue
    }

    public enum StatutCandidature
    {
        Soumise,
        EnAnalyse,
        Convoquee,
        Acceptee,
        Refusee,
        EnCours,
        Cloturee
    }

    public enum StatutConvention
    {
        EnAttente,
        Signe
    }

    public enum ExpediteurMessage
    {
        Etudiant,
        Encadrant
    }
}
