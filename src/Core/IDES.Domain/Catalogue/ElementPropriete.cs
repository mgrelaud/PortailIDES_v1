// Dans Models/Catalogue/ElementPropriete.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDES.Domain.Catalogue
{
    /// <summary>
    /// Représente l'association entre un DefinitionElement et une DefinitionPropriete.
    /// C'est cette entité qui porte la valeur/formule spécifique à cette association.
    /// </summary>
    public class ElementPropriete
    {
        [Key]
        public int Id { get; set; }

        // --- Clés étrangères ---
        public int DefinitionElementId { get; set; }
        [ForeignKey(nameof(DefinitionElementId))]
        public virtual DefinitionElement DefinitionElement { get; set; } = null!;

        public int DefinitionProprieteId { get; set; }
        [ForeignKey(nameof(DefinitionProprieteId))]
        public virtual DefinitionPropriete DefinitionPropriete { get; set; } = null!;

        // --- Donnée spécifique à l'association ---

        /// <summary>
        /// La valeur ou la formule spécifique pour cette propriété DANS LE CONTEXTE de cet élément.
        /// Ex: Pour la propriété "Hauteur" sur l'élément "Poutre", la valeur pourrait être "0.5".
        /// Ex: Pour la propriété "Volume" sur l'élément "Poutre", la valeur pourrait être "{Hauteur}*{Largeur}".
        /// </summary>
        public string? Valeur { get; set; }
    }
}
