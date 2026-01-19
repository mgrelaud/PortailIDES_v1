using IDES.Application.Interfaces;
using IDES.Domain.Metre;
using IDES.Domain.Proprietes;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IDES.Infrastructure.Services;

public class MoteurCalculService : IMoteurCalculService
{
    public object? Evaluer(string? formule, ElementDynamique element)
    {
        var contexte = new Dictionary<string, object>();

        foreach (var prop in element.Proprietes)
        {
            if (double.TryParse(prop.ValeurObjet?.ToString(), out double valNum))
            {
                contexte[prop.Nom] = valNum;
            }
            else if (prop.ValeurObjet is bool valBool)
            {
                contexte[prop.Nom] = valBool;
            }
        }

        contexte["VolumeBeton"] = EvaluerAvecContexte(element.FormuleBeton, contexte);
        contexte["TotalCoffrage"] = EvaluerAvecContexte(element.FormuleCoffrage, contexte);
        contexte["TotalAcierHA"] = EvaluerAvecContexte(element.FormuleAcierHA, contexte);
        contexte["TotalAcierTS"] = EvaluerAvecContexte(element.FormuleAcierTS, contexte);

        return EvaluerAvecContexte(formule, contexte);
    }

    private object EvaluerAvecContexte(string? formule, Dictionary<string, object> contexte)
    {
        if (string.IsNullOrWhiteSpace(formule))
        {
            return 0.0;
        }

        string formuleTransformee = PreTransformerFormule(formule);
        var expression = new Expression(formuleTransformee);
        expression.Parameters = contexte;

        try
        {
            object resultat = expression.Evaluate();
            return Convert.ToDouble(resultat);
        }
        catch
        {
            return formule;
        }
    }

    private string PreTransformerFormule(string formule)
    {
        formule = formule.Replace("{", "").Replace("}", "");
        formule = Regex.Replace(formule, @"([\w\.]+)\s*\^\s*([\d\.]+)", "Pow($1, $2)");
        formule = Regex.Replace(formule, "PI", "Pi", RegexOptions.IgnoreCase);
        return formule;
    }
}
