namespace PortailMetier.Domain.Dtos;

public class FichierDto
{
    public string Nom { get; set; } = string.Empty;
    public string TailleHumaine { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime DateModifUtc { get; set; }
    public string CheminCompletUNC { get; set; } = string.Empty;
}
