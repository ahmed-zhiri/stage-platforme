using AutoMapper;
using GestionStagiaires.Web.DTOs;
using GestionStagiaires.Web.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GestionStagiaires.Web.Mapping;

/// <summary>
/// Profil AutoMapper centralisant les correspondances Entité &lt;-&gt; DTO / ViewModel.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entité -> DTO de lecture
        CreateMap<Stagiaire, StagiaireDto>()
            .ForMember(d => d.NomComplet, o => o.MapFrom(s => s.NomComplet))
            .ForMember(d => d.StatutLibelle, o => o.MapFrom(s => GetDisplayName(s.Statut)));

        // DTO de création -> Entité
        CreateMap<StagiaireCreateDto, Stagiaire>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore());

        // DTO de mise à jour -> Entité (les champs d'audit sont gérés dans le service)
        CreateMap<StagiaireUpdateDto, Stagiaire>()
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore());

        // Entité -> DTO de mise à jour (pré-remplissage du formulaire d'édition)
        CreateMap<Stagiaire, StagiaireUpdateDto>();
    }

    /// <summary>
    /// Récupère le libellé [Display(Name=...)] associé à une valeur d'enum.
    /// </summary>
    private static string GetDisplayName(Enum value)
    {
        MemberInfo? member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        DisplayAttribute? attr = member?.GetCustomAttribute<DisplayAttribute>();
        return attr?.Name ?? value.ToString();
    }
}
