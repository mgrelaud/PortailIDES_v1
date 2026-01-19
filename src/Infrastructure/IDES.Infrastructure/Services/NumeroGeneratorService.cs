using IDES.Application.Interfaces;
using IDES.Domain;
using System.Collections.Generic;

namespace IDES.Infrastructure.Services;

public class NumeroGeneratorService : INumeroGeneratorService
{
    private readonly IConfigService _configService;

    public NumeroGeneratorService(IConfigService configService)
    {
        _configService = configService;
    }

    public string GenererProchainNumero(TypeTitre typeTitre, IEnumerable<object> metre)
    {
        var config = _configService.Config;
        int dernierCompteurTitre = 0;
        int dernierCompteurSousTitre = 0;
        int dernierCompteurSousSousTitre = 0;

        string dernierNumeroTitreParent = "";
        string dernierNumeroSousTitreParent = "";

        foreach (var element in metre)
        {
            if (element is TitreElement titre)
            {
                switch (titre.TypeTitre)
                {
                    case TypeTitre.Titre:
                        dernierCompteurTitre++;
                        dernierCompteurSousTitre = 0;
                        dernierCompteurSousSousTitre = 0;
                        dernierNumeroTitreParent = GenererNumeroSimple(config.TypeTitre, dernierCompteurTitre);
                        break;

                    case TypeTitre.SousTitre:
                        dernierCompteurSousTitre++;
                        dernierCompteurSousSousTitre = 0;
                        dernierNumeroSousTitreParent = GenererNumeroSimple(config.TypeSousTitre, dernierCompteurSousTitre);
                        break;

                    case TypeTitre.SousSousTitre:
                        dernierCompteurSousSousTitre++;
                        break;
                }
            }
        }

        switch (typeTitre)
        {
            case TypeTitre.Titre:
                dernierCompteurTitre++;
                string numeroTitre = GenererNumeroSimple(config.TypeTitre, dernierCompteurTitre);
                return numeroTitre + config.TerminaisonTitre;

            case TypeTitre.SousTitre:
                dernierCompteurSousTitre++;
                string numeroSousTitre = GenererNumeroSimple(config.TypeSousTitre, dernierCompteurSousTitre);
                return dernierNumeroTitreParent + config.SeparateurSousTitre + numeroSousTitre + config.TerminaisonSousTitre;

            case TypeTitre.SousSousTitre:
                dernierCompteurSousSousTitre++;
                string numeroSousSousTitre = GenererNumeroSimple(config.TypeSousSousTitre, dernierCompteurSousSousTitre);
                return dernierNumeroTitreParent + config.SeparateurSousTitre + dernierNumeroSousTitreParent +
                       config.SeparateurSousSousTitre + numeroSousSousTitre + config.TerminaisonSousSousTitre;

            default:
                return "";
        }
    }

    private string GenererNumeroSimple(string type, int compteur)
    {
        switch (type)
        {
            case "A":
                return ((char)('A' + compteur - 1)).ToString();
            case "a":
                return ((char)('a' + compteur - 1)).ToString();
            case "I":
                return ConvertirEnRomainMajuscule(compteur);
            case "i":
                return ConvertirEnRomainMinuscule(compteur);
            case "1":
            default:
                return compteur.ToString();
        }
    }

    private string ConvertirEnRomainMajuscule(int nombre)
    {
        if (nombre <= 0) return "";
        if (nombre >= 4000) return nombre.ToString();

        string[] milliers = { "", "M", "MM", "MMM" };
        string[] centaines = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
        string[] dizaines = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
        string[] unites = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

        return milliers[nombre / 1000] +
               centaines[(nombre % 1000) / 100] +
               dizaines[(nombre % 100) / 10] +
               unites[nombre % 10];
    }

    private string ConvertirEnRomainMinuscule(int nombre)
    {
        return ConvertirEnRomainMajuscule(nombre).ToLower();
    }
}
