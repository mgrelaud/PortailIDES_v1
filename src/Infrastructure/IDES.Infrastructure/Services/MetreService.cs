using IDES.Application.Interfaces;
using IDES.Domain;
using IDES.Domain.Metre;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IDES.Infrastructure.Services;

public class MetreService : IMetreService
{
    private readonly INumeroGeneratorService _numeroGenerator;
    private readonly IConfigService _configService;

    public ObservableCollection<object> Metre { get; } = new();

    public MetreService(INumeroGeneratorService numeroGenerator, IConfigService configService)
    {
        _numeroGenerator = numeroGenerator;
        _configService = configService;
    }

    public void AjouterElement(ElementDynamique element)
    {
        Metre.Add(element);
        RecalculerTout();
    }

    public void AjouterTitre(TitreElement titre)
    {
        titre.Numero = _numeroGenerator.GenererProchainNumero(titre.TypeTitre, Metre);
        Metre.Add(titre);
        RecalculerTout();
    }

    public void SupprimerElement(object element)
    {
        Metre.Remove(element);
        RecalculerTout();
    }

    public void DeplacerHaut(object element)
    {
        int index = Metre.IndexOf(element);
        if (index > 0)
        {
            Metre.Move(index, index - 1);
            RecalculerTout();
        }
    }

    public void DeplacerBas(object element)
    {
        int index = Metre.IndexOf(element);
        if (index < Metre.Count - 1)
        {
            Metre.Move(index, index + 1);
            RecalculerTout();
        }
    }

    public double TotalGeneralBeton => Metre.OfType<ElementDynamique>().Sum(e => ConvertToDouble(e.VolumeBeton));
    public double TotalGeneralAcier => Metre.OfType<ElementDynamique>().Sum(e => ConvertToDouble(e.TotalAcierHA) + ConvertToDouble(e.TotalAcierTS));
    public double TotalGeneralCoffrage => Metre.OfType<ElementDynamique>().Sum(e => ConvertToDouble(e.TotalCoffrage));

    public void RecalculerTout()
    {
        // Recalculer la numérotation
        var elementsParcourus = new List<object>();
        foreach (var element in Metre)
        {
            if (element is TitreElement titre)
            {
                titre.Numero = _numeroGenerator.GenererProchainNumero(titre.TypeTitre, elementsParcourus);
            }
            elementsParcourus.Add(element);
        }
    }

    private double ConvertToDouble(object? value)
    {
        try
        {
            return Convert.ToDouble(value);
        }
        catch
        {
            return 0.0;
        }
    }
}
