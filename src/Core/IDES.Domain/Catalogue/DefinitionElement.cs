// Assurez-vous que le namespace est bien celui-ci
namespace IDES.Domain.Catalogue
{
    public class DefinitionElement
    {
        public int Id { get; set; }

        // On initialise les chaînes à 'string.Empty' pour faire taire les avertissements.
        public string Nom { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
        public string PrefixeRepere { get; set; } = string.Empty;
        public string DesignationTemplate { get; set; } = string.Empty;
        public DateTime DateCreation { get; set; }
        public string CreePar { get; set; } = string.Empty;
        public DateTime DateModification { get; set; }
        public string ModifiePar { get; set; } = string.Empty;

        // La collection est déjà initialisée, c'est parfait.
        //public ICollection<DefinitionPropriete> Proprietes { get; set; } = new List<DefinitionPropriete>();
        /// <summary>
        /// Collection des associations de propriétés pour cet élément.
        /// C'est ici que sont stockées les valeurs/formules spécifiques.
        /// </summary>
        public virtual ICollection<ElementPropriete> ProprietesAssociees { get; set; } = new List<ElementPropriete>();
    }
}
