using CommunityToolkit.Mvvm.ComponentModel;
using IDES.Domain.Proprietes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace IDES.Domain.Metre
{
    public partial class ElementDynamique : ObservableObject
    {
        //==================================================================
        // PROPRIÉTÉS DE BASE
        //==================================================================
        private readonly ElementCalculator _calculator;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Designation))]
        private string _nomElement = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Designation))]
        private string _repere = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Designation))]
        private string _designationTemplate = string.Empty;

        //==================================================================
        // COLLECTION DE PROPRIÉTÉS
        //==================================================================
        public ObservableCollection<Propriete> Proprietes { get; } = new();

        public ElementDynamique()
        {
            // On crée une instance du calculateur et on lui passe "this" (cet élément).
            _calculator = new ElementCalculator(this);

            // On s'abonne aux changements de la collection elle-même (pour les ajouts/suppressions)
            Proprietes.CollectionChanged += Proprietes_CollectionChanged;
        }

        /// <summary>
        /// Cette méthode est appelée quand une propriété est ajoutée ou retirée de la collection.
        /// </summary>
        private void Proprietes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Si de nouvelles propriétés sont ajoutées, on s'abonne à leurs changements internes.
            if (e.NewItems != null)
            {
                foreach (Propriete newItem in e.NewItems)
                {
                    newItem.PropertyChanged += OnValeurProprieteChanged;
                }
            }

            // Si des propriétés sont retirées, on se désabonne pour éviter les fuites de mémoire.
            if (e.OldItems != null)
            {
                foreach (Propriete oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= OnValeurProprieteChanged;
                }
            }
        }

        //==================================================================
        // ACCESSEURS SÉCURISÉS AUX PROPRIÉTÉS
        //==================================================================
        private void OnValeurProprieteChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Valeur" || e.PropertyName == "ValeurObjet")
            {
                // On notifie le calculateur que le cache n'est plus valide.
                _calculator.InvalidateCache();

                // On notifie l'UI que toutes les propriétés liées à cet objet doivent être rafraîchies.
                OnPropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// Méthode privée pour trouver la formule d'une propriété système par son nom.
        /// </summary>
        private string? GetSystemFormula(string nomPropriete)
        {
            return Proprietes.OfType<ProprieteFormule>()
                             .FirstOrDefault(p => p.Nom == nomPropriete)?
                             .Formule;
        }

        // On remplace les anciennes propriétés par des nouvelles qui cherchent dynamiquement la formule.
        public string? FormuleBeton => GetSystemFormula("VolumeBeton");
        public string? FormuleAcierHA => GetSystemFormula("PoidsHA");
        public string? FormuleAcierTS => GetSystemFormula("PoidsTS");
        public string? FormuleCoffrage => GetSystemFormula("SurfaceCoffrage");
        public string? FormuleAvantMetre => GetSystemFormula("AvantMetre"); // Assurez-vous que le nom est correct

        //==================================================================
        // PROPRIÉTÉS CALCULÉES (ne changent pas, car elles dépendent des propriétés de formules ci-dessus)
        //==================================================================

        public string Designation => RemplacerJetonsDynamique(this.DesignationTemplate);
        public object? AvantMetre => _calculator.GetValue("AvantMetre");
        public object? VolumeBeton => _calculator.GetValue("VolumeBeton");
        public object? TotalCoffrage => _calculator.GetValue("TotalCoffrage");
        public object? TotalAcierHA => _calculator.GetValue("TotalAcierHA");
        public object? TotalAcierTS => _calculator.GetValue("TotalAcierTS");

        // =================================================================
        // PROPRIÉTÉS DE PRÉSENTATION POUR LA DATAGRID
        // =================================================================

        /// <summary>
        /// Propriété pour la colonne "AV METRE" (Quantité).
        /// </summary>
        public object? AvantMetreQuantite
        {
            get
            {
                // On récupère le résultat du calcul.
                var resultat = this.AvantMetre;

                // Si le résultat est un nombre, on le retourne.
                if (resultat is double || resultat is int || resultat is decimal)
                {
                    return resultat;
                }

                // Sinon (si c'est du texte comme "Forfait"), on ne retourne rien pour la quantité.
                return null;
            }
        }

        /// <summary>
        /// Propriété pour la colonne "AV METRE" (Unité).
        /// </summary>
        public string? AvantMetreUnite
        {
            get
            {
                var resultat = this.AvantMetre;

                // Si le résultat est un nombre, on essaie de trouver l'unité.
                if (resultat is double || resultat is int || resultat is decimal)
                {
                    string? formule = this.FormuleAvantMetre?.Trim();

                    // Scénario 1 : La formule est un jeton unique (ex: "{Longueur}")
                    if (!string.IsNullOrEmpty(formule) && formule.StartsWith("{") && formule.EndsWith("}"))
                    {
                        // On extrait le nom de la propriété (ex: "Longueur")
                        string nomPropriete = formule.Substring(1, formule.Length - 2);

                        // On cherche la propriété correspondante dans notre collection
                        var proprieteSource = this.Proprietes.FirstOrDefault(p => p.Nom == nomPropriete);

                        // Si on l'a trouvée et qu'elle a une unité, on la retourne !
                        if (proprieteSource != null && !string.IsNullOrEmpty(proprieteSource.Unite))
                        {
                            return proprieteSource.Unite;
                        }
                    }

                    // Scénario 2 (fallback) : La formule est complexe ou on n'a pas trouvé d'unité.
                    // On retourne une unité générique ou rien.
                    return string.Empty; // ou string.Empty si vous préférez
                }

                // Si le résultat est du texte (ex: "Forfait"), on l'affiche à la place de l'unité.
                return resultat?.ToString();
            }
        }

        /// <summary>
        /// Propriété pour la colonne "COFFRAGE".
        /// Gère à la fois les nombres et le texte.
        /// </summary>
        public string AffichageCoffrage
        {
            get
            {
                var resultat = this.TotalCoffrage;

                // Si le résultat est un nombre, on le formate avec 2 décimales.
                if (resultat is double val)
                {
                    return val.ToString("N2");
                }

                // Sinon, on retourne le texte tel quel (ex: "Dalle").
                return resultat?.ToString() ?? string.Empty;
            }
        }

        //==================================================================
        // MÉTHODE DE REMPLACEMENT DES JETONS
        //==================================================================

        private string RemplacerJetonsDynamique(string template)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            var sb = new StringBuilder(template);

            sb.Replace("{Nom}", this.NomElement);
            sb.Replace("{Repere}", this.Repere);

            foreach (var prop in Proprietes)
            {
                string jeton = $"{{{prop.Nom}}}";
                string valeurFormatee = "";

                // On utilise le pattern matching sur les types pour un formatage correct
                if (prop is ProprieteDouble pDouble && pDouble.Valeur is double valD)
                {
                    valeurFormatee = valD.ToString("N2"); // Format à 2 décimales
                }
                else if (prop is ProprieteBool pBool && pBool.Valeur is bool valB)
                {
                    valeurFormatee = valB ? "Oui" : "Non";
                }
                else if (prop.ValeurObjet != null)
                {
                    valeurFormatee = prop.ValeurObjet.ToString() ?? "";
                }

                sb.Replace(jeton, valeurFormatee);
            }

            return sb.ToString();
        }
    }
}
