// Dans Models/Proprietes/ProprieteFormule.cs
namespace IDES.Domain.Proprietes
{
    /// <summary>
    /// Représente une propriété dont la valeur est le résultat d'une formule.
    /// Cette classe ne contient pas de 'Valeur' directement, mais la formule pour la calculer.
    /// </summary>
    public partial class ProprieteFormule : Propriete
    {
        /// <summary>
        /// La chaîne de caractères représentant la formule à évaluer (ex: "{Largeur}*{Hauteur}").
        /// </summary>
        public string? Formule { get; set; }

        /// <summary>
        /// Pour une ProprieteFormule, la 'Valeur' n'est pas stockée mais calculée à la volée.
        /// On retourne la formule elle-même pour l'affichage ou le débogage.
        /// Le setter ne fait rien car la valeur est dynamique.
        /// </summary>
        public override object? ValeurObjet
        {
            get => Formule; // Retourne la formule pour l'affichage
            set
            {
                // Le setter est intentionnellement vide ou simple.
                // La valeur est calculée, pas définie directement.
                if (value is string str)
                {
                    Formule = str;
                }
            }
        }

        public ProprieteFormule(string nom, string nomAffichage)
            : base(nom, nomAffichage)
        {
            // Le constructeur de base est suffisant.
        }
    }
}
