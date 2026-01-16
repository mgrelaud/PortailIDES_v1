namespace PortailMetier.Domain.Dtos;

public class DossierDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Nom { get; set; } = string.Empty;
    public string CheminCompletUNC { get; set; } = string.Empty;
    public bool HasContent { get; set; } = false;
    public List<DossierDto> SousDossiers { get; set; } = new();
}