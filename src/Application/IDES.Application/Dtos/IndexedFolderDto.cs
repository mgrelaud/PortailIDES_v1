namespace IDES.Application.Dtos;

/// <summary>
/// Représente un dossier indexé dans la base de données
/// </summary>
public class IndexedFolderDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string CheminCompletUNC { get; set; } = string.Empty;
    public string? ParentPath { get; set; }
    public bool HasContent { get; set; }
    public DateTime DateIndexation { get; set; }
    public DateTime? DateModification { get; set; }
}
