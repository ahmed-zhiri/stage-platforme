using Microsoft.AspNetCore.Identity;

namespace GestionStagiaires.Web.Models.Identity;

/// <summary>
/// Utilisateur applicatif étendant IdentityUser afin de pouvoir
/// ajouter des propriétés métier (nom complet, etc.).
/// </summary>
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string? NomComplet { get; set; }
}
