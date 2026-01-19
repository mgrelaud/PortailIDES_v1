// Fichier : Models/Proprietes/ProprieteGenerique.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace IDES.Domain.Proprietes
{
    // T est le type de la valeur (double, string, bool, etc.)
    public abstract partial class Propriete<T> : Propriete
    {
        // Votre propriété générique existante, renommée pour éviter la confusion.
        // Le [ObservableProperty] va générer une propriété nommée "ValeurGenerique"
        //[ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(Valeur))] // Notifie que la propriété non-générique a aussi changé
        //private T _valeurGenerique;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ValeurObjet))]
        private T _valeur;

        // Implémentation de la propriété abstraite de la classe de base
        //public override object? Valeur
        //{
        //    get => ValeurGenerique;
        //    set
        //    {
        //        // On ne tente la conversion que si la valeur n'est pas null
        //        if (value != null)
        //        {
        //            try
        //            {
        //                // On utilise le type de la valeur générique actuelle comme cible
        //                // C'est plus sûr que typeof(T) dans certains contextes.
        //                Type targetType = typeof(T);
        //                T convertedValue = (T)Convert.ChangeType(value, targetType);
        //                ValeurGenerique = convertedValue;
        //            }
        //            catch
        //            {
        //                // La conversion a échoué, on ne fait rien.
        //            }
        //        }
        //        else
        //        {
        //            // Si la valeur reçue est null, on assigne la valeur par défaut de T
        //            ValeurGenerique = default!;
        //        }
        //    }
        //}
        public override object? ValeurObjet
        {
            get => this.Valeur; // Lit depuis la propriété générée 'Valeur'
            set
            {
                try
                {
                    // Tente de convertir la valeur reçue (de type object) vers le type T
                    if (value != null)
                    {
                        // On assigne à la propriété générée 'Valeur', qui va déclencher la notification.
                        this.Valeur = (T)Convert.ChangeType(value, typeof(T));
                    }
                    else
                    {
                        this.Valeur = default!;
                    }
                }
                catch
                {
                    // En cas d'échec de conversion (ex: taper "abc" dans un champ numérique),
                    // on ne change pas la valeur.
                }
            }
        }

        protected Propriete(string nom, string nomAffichage, T valeurParDefaut)
            : base(nom, nomAffichage)
        {
            //_valeurGenerique = valeurParDefaut;
            _valeur = valeurParDefaut;
        }
    }
}
