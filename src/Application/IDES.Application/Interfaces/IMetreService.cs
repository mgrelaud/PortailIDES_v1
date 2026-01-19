using IDES.Domain.Metre;
using IDES.Domain;
using System.Collections.ObjectModel;

namespace IDES.Application.Interfaces;

public interface IMetreService
{
    ObservableCollection<object> Metre { get; }
    
    void AjouterElement(ElementDynamique element);
    void AjouterTitre(TitreElement titre);
    void SupprimerElement(object element);
    void DeplacerHaut(object element);
    void DeplacerBas(object element);
    
    double TotalGeneralBeton { get; }
    double TotalGeneralAcier { get; }
    double TotalGeneralCoffrage { get; }
    
    void RecalculerTout();
}
