using AutoMapper;
using GestionStagiaires.Web.Data;
using GestionStagiaires.Web.DTOs;
using GestionStagiaires.Web.Models.Entities;
using GestionStagiaires.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Web.Services;

/// <summary>
/// Implémentation de la couche métier : orchestre l'accès aux données via
/// le DbContext EF Core et le mapping via AutoMapper.
/// </summary>
public class StagiaireService : IStagiaireService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public StagiaireService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<StagiaireDto>> GetPagedAsync(
        string? recherche,
        StatutStage? statut,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        IQueryable<Stagiaire> query = _context.Stagiaires.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(recherche))
        {
            string terme = recherche.Trim();
            query = query.Where(s =>
                EF.Functions.Like(s.Nom, $"%{terme}%") ||
                EF.Functions.Like(s.Prenom, $"%{terme}%") ||
                EF.Functions.Like(s.Email, $"%{terme}%") ||
                EF.Functions.Like(s.Etablissement, $"%{terme}%"));
        }

        if (statut.HasValue)
        {
            query = query.Where(s => s.Statut == statut.Value);
        }

        int totalItems = await query.CountAsync(cancellationToken);

        List<Stagiaire> entities = await query
            .OrderByDescending(s => s.CreatedAt)
            .ThenBy(s => s.Nom)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Mapping en mémoire (le libellé du statut provient d'un attribut [Display]).
        List<StagiaireDto> items = _mapper.Map<List<StagiaireDto>>(entities);

        return new PagedResult<StagiaireDto>(items, totalItems, pageNumber, pageSize);
    }

    public async Task<StagiaireDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        Stagiaire? entity = await _context.Stagiaires
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return entity is null ? null : _mapper.Map<StagiaireDto>(entity);
    }

    public async Task<StagiaireUpdateDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        Stagiaire? entity = await _context.Stagiaires
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return entity is null ? null : _mapper.Map<StagiaireUpdateDto>(entity);
    }

    public async Task<StagiaireDto> CreateAsync(StagiaireCreateDto dto, CancellationToken cancellationToken = default)
    {
        Stagiaire entity = _mapper.Map<Stagiaire>(dto);
        entity.CreatedAt = DateTime.UtcNow;

        _context.Stagiaires.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<StagiaireDto>(entity);
    }

    public async Task<StagiaireDto?> UpdateAsync(StagiaireUpdateDto dto, CancellationToken cancellationToken = default)
    {
        Stagiaire? entity = await _context.Stagiaires
            .FirstOrDefaultAsync(s => s.Id == dto.Id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        // Conserve la date de création, met à jour les autres champs.
        DateTime createdAt = entity.CreatedAt;
        _mapper.Map(dto, entity);
        entity.CreatedAt = createdAt;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<StagiaireDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Stagiaire? entity = await _context.Stagiaires
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        _context.Stagiaires.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        string normalized = email.Trim();
        return _context.Stagiaires.AnyAsync(
            s => s.Email == normalized && (excludeId == null || s.Id != excludeId),
            cancellationToken);
    }
}
