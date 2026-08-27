using System.ComponentModel.DataAnnotations;

namespace GestionStagiaires.Web.DTOs;

/// <summary>
/// DTO / ViewModel de mise à jour. Hérite des champs de création et
/// ajoute l'identifiant de l'entité à modifier.
/// </summary>
public class StagiaireUpdateDto : StagiaireCreateDto
{
    [Required]
    public int Id { get; set; }
}
