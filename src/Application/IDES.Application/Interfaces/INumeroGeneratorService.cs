using IDES.Domain;

namespace IDES.Application.Interfaces;

public interface INumeroGeneratorService
{
    string GenererProchainNumero(TypeTitre typeTitre, IEnumerable<object> metre);
}
