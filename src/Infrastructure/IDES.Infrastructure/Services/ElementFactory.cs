using IDES.Application.Interfaces;
using IDES.Domain.Catalogue;
using IDES.Domain.Metre;
using IDES.Domain.Proprietes;
using System;
using System.Linq;

namespace IDES.Infrastructure.Services;

public class ElementFactory : IElementFactory
{
    public ElementDynamique CreerElementDynamique(DefinitionElement definition)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        var nouvelElement = new ElementDynamique
        {
            NomElement = definition.Nom,
            Repere = definition.PrefixeRepere,
            DesignationTemplate = definition.DesignationTemplate
        };

        if (definition.ProprietesAssociees != null && definition.ProprietesAssociees.Any())
        {
            foreach (var association in definition.ProprietesAssociees)
            {
                var propDef = association.DefinitionPropriete;
                var valeurSpecifique = association.Valeur;

                Propriete? nouvellePropriete = propDef.TypeDonnee?.ToLower() switch
                {
                    "double" => new ProprieteDouble(propDef.Nom, propDef.NomAffichage),
                    "string" => new ProprieteString(propDef.Nom, propDef.NomAffichage),
                    "bool" => new ProprieteBool(propDef.Nom, propDef.NomAffichage),
                    "formule" => new ProprieteFormule(propDef.Nom, propDef.NomAffichage),
                    _ => null
                };

                if (nouvellePropriete != null)
                {
                    nouvellePropriete.Unite = propDef.Unite;

                    if (nouvellePropriete is ProprieteFormule propFormule)
                    {
                        propFormule.Formule = valeurSpecifique;
                    }
                    else if (!string.IsNullOrEmpty(valeurSpecifique))
                    {
                        try
                        {
                            var targetType = nouvellePropriete.GetType().GetProperty("Valeur")?.PropertyType;
                            if (targetType != null)
                            {
                                object convertedValue = Convert.ChangeType(valeurSpecifique, targetType);
                                nouvellePropriete.ValeurObjet = convertedValue;
                            }
                        }
                        catch { }
                    }

                    nouvelElement.Proprietes.Add(nouvellePropriete);
                }
            }
        }

        return nouvelElement;
    }
}
