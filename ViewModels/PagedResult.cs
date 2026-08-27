namespace GestionStagiaires.Web.ViewModels;

/// <summary>
/// Résultat paginé générique réutilisable (pagination).
/// </summary>
/// <typeparam name="T">Type des éléments de la page.</typeparam>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>Numéro de la page courante (base 1).</summary>
    public int PageNumber { get; set; }

    /// <summary>Nombre d'éléments par page.</summary>
    public int PageSize { get; set; }

    /// <summary>Nombre total d'éléments (toutes pages confondues).</summary>
    public int TotalItems { get; set; }

    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling(TotalItems / (double)PageSize)
        : 0;

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResult() { }

    public PagedResult(IReadOnlyList<T> items, int totalItems, int pageNumber, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
