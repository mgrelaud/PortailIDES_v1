// Assurez-vous que le namespace est bien celui-ci
namespace IDES.Domain.Catalogue
{
    public class DefinitionPropriete
    {
        public int Id { get; set; }

        // On initialise les chaînes à 'string.Empty'
        public string Nom { get; set; } = string.Empty;
        public string NomAffichage { get; set; } = string.Empty;
        public string TypeDonnee { get; set; } = string.Empty;

        // Les propriétés nullables n'ont pas besoin d'être initialisées.
        public string? Unite { get; set; }
        public string? Categorie { get; set; }

        // 2. La "valeur" de la propriété.
        //    - Si TypeDonnee = "Double", contient la valeur par défaut (ex: "0.0").
        //    - Si TypeDonnee = "Formule", CONTIENT LA FORMULE ELLE-MÊME (ex: "{Largeur}*{Hauteur}").
        public string? ValeurParDefaut { get; set; }

        // 4. Le format d'affichage, pour les cas spéciaux comme les arases.
        public string? FormatAffichage { get; set; } // ex: "N3", "Arase"

        public DateTime DateCreation { get; set; }
        public string CreePar { get; set; } = string.Empty;
        public DateTime DateModification { get; set; }
        public string ModifiePar { get; set; } = string.Empty;

        //public ICollection<DefinitionElement> TypesElements { get; set; } = new List<DefinitionElement>();
        // === À AJOUTER ===
        /// <summary>
        /// Collection des associations où cette propriété est utilisée.
        /// </summary>
        public virtual ICollection<ElementPropriete> ElementsAssocies { get; set; } = new List<ElementPropriete>();

        public override string ToString() => NomAffichage;
    }
}
