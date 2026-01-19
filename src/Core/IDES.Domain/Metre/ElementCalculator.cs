// Dans Services/ElementCalculator.cs
using NCalc;
using IDES.Domain.Metre;
using IDES.Domain.Proprietes;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IDES.Domain.Metre
{
    /// <summary>
    /// Gère tous les calculs pour UN SEUL ElementDynamique.
    /// Met en cache les résultats pour éviter les recalculs inutiles.
    /// </summary>
    public class ElementCalculator
    {
        private readonly ElementDynamique _element;
        private readonly Dictionary<string, object> _resultsCache = new Dictionary<string, object>();

        public ElementCalculator(ElementDynamique element)
        {
            _element = element;
            // On s'abonne aux changements de l'élément pour savoir quand vider le cache.
            _element.PropertyChanged += (s, e) => InvalidateCacheIfNeeded(e.PropertyName);
        }

        /// <summary>
        /// Vide le cache. Appelé quand une valeur de base (comme Largeur) change.
        /// </summary>
        public void InvalidateCache()
        {
            _resultsCache.Clear();
        }

        /// <summary>
        /// Vide le cache uniquement si une propriété pertinente a changé.
        /// Pour l'instant, on est simple : on vide toujours le cache.
        /// </summary>
        private void InvalidateCacheIfNeeded(string? propertyName)
        {
            // On pourrait avoir une logique plus fine ici, mais pour l'instant,
            // vider le cache à chaque notification est sûr et déjà beaucoup plus performant.
            InvalidateCache();
        }

        /// <summary>
        /// Le point d'entrée public pour obtenir la valeur d'une formule (ex: "VolumeBeton").
        /// Utilise le cache si la valeur a déjà été calculée.
        /// </summary>
        /// 
        public object? GetValue(string nomFormule)
        {
            if (_resultsCache.TryGetValue(nomFormule, out var cachedValue))
            {
                return cachedValue;
            }

            string? formule = GetFormuleByName(nomFormule);
            if (string.IsNullOrWhiteSpace(formule))
            {
                _resultsCache[nomFormule] = 0.0;
                return 0.0;
            }

            // =================================================================
            // NOUVELLE LOGIQUE DE DÉTECTION
            // On vérifie si la formule est juste un mot simple sans opérateurs,
            // accolades ou chiffres. C'est probablement du texte pur.
            // =================================================================
            bool estProbablementDuTexte = !formule.Any(c => "{}()+-*/^".Contains(c)) && formule.All(c => !char.IsDigit(c));

            if (estProbablementDuTexte)
            {
                _resultsCache[nomFormule] = formule; // On met en cache le texte et on le retourne directement
                return formule;
            }

            // Si ce n'est pas du texte pur, on continue avec le calcul...
            var contexte = new Dictionary<string, object>();
            foreach (var prop in _element.Proprietes)
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

            var expression = new Expression(PreTransformerFormule(formule));

            expression.EvaluateParameter += (name, args) =>
            {
                args.Result = GetValue(name);
            };

            expression.Parameters = contexte;

            try
            {
                object resultat = expression.Evaluate();
                object finalValue = Convert.ToDouble(resultat);
                _resultsCache[nomFormule] = finalValue;
                return finalValue;
            }
            catch
            {
                // Ce catch devient un filet de sécurité. Normalement, on ne devrait plus y passer pour du texte pur.
                _resultsCache[nomFormule] = formule;
                return formule;
            }
        }
        //public object? GetValue(string nomFormule)
        //{
        //    // Si la valeur est déjà en cache, on la retourne immédiatement. C'est la clé de la performance.
        //    if (_resultsCache.TryGetValue(nomFormule, out var cachedValue))
        //    {
        //        return cachedValue;
        //    }

        //    // Sinon, on doit la calculer.
        //    string? formule = GetFormuleByName(nomFormule);
        //    if (string.IsNullOrWhiteSpace(formule))
        //    {
        //        _resultsCache[nomFormule] = 0.0; // Mettre en cache la valeur par défaut
        //        return 0.0;
        //    }

        //    // Préparation du contexte (uniquement les propriétés de base de l'élément)
        //    var contexte = new Dictionary<string, object>();
        //    foreach (var prop in _element.Proprietes)
        //    {
        //        if (double.TryParse(prop.ValeurObjet?.ToString(), out double valNum))
        //        {
        //            contexte[prop.Nom] = valNum;
        //        }
        //        else if (prop.ValeurObjet is bool valBool)
        //        {
        //            contexte[prop.Nom] = valBool;
        //        }
        //    }

        //    var expression = new Expression(PreTransformerFormule(formule));

        //    // ASTUCE PUISSANTE : On dit à NCalc comment trouver les valeurs qu'il ne connaît pas.
        //    // C'est ce qui gère les calculs en cascade (dépendances entre formules).
        //    expression.EvaluateParameter += (name, args) =>
        //    {
        //        // Si NCalc demande la valeur de "VolumeBeton" dans une autre formule,
        //        // on appelle récursivement GetValue pour la calculer (ou la récupérer du cache).
        //        args.Result = GetValue(name);
        //    };

        //    // On fournit les paramètres de base que nous avons préparés.
        //    expression.Parameters = contexte;

        //    try
        //    {
        //        object resultat = expression.Evaluate();
        //        object finalValue = Convert.ToDouble(resultat);
        //        _resultsCache[nomFormule] = finalValue; // On stocke le résultat dans le cache.
        //        return finalValue;
        //    }
        //    catch
        //    {
        //        // Si le calcul échoue (texte pur, erreur de syntaxe), on met en cache le texte.
        //        _resultsCache[nomFormule] = formule;
        //        return formule;
        //    }
        //}

        /// <summary>
        /// Fait le lien entre un nom de formule et la propriété string qui la contient dans ElementDynamique.
        /// </summary>
        private string? GetFormuleByName(string name) => name switch
        {
            "VolumeBeton" => _element.FormuleBeton,
            "TotalCoffrage" => _element.FormuleCoffrage,
            "TotalAcierHA" => _element.FormuleAcierHA,
            "TotalAcierTS" => _element.FormuleAcierTS,
            "AvantMetre" => _element.FormuleAvantMetre,
            // Ajoutez d'autres formules ici si nécessaire
            _ => null // Si le nom n'est pas reconnu, on retourne null.
        };

        /// <summary>
        /// Nettoie et transforme une formule pour la rendre compatible avec la syntaxe de NCalc.
        /// </summary>
        private string PreTransformerFormule(string formule)
        {
            formule = formule.Replace("{", "").Replace("}", "");
            formule = Regex.Replace(formule, @"([\w\.]+)\s*\^\s*([\d\.]+)", "Pow($1, $2)");
            formule = Regex.Replace(formule, "PI", "Pi", RegexOptions.IgnoreCase);
            return formule;
        }
    }
}
