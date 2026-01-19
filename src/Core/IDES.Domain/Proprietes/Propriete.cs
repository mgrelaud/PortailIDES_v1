// Fichier : Models/Proprietes/Propriete.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace IDES.Domain.Proprietes
{
    // partial pour que le toolkit puisse générer du code
    public abstract partial class Propriete : ObservableObject
    {
        // Le nom technique de la propriété (ex: "Largeur", "RatioAcierHA")
        [ObservableProperty]
        private string _nom;

        // Le nom affiché à l'utilisateur (ex: "Largeur (m)")
        [ObservableProperty]
        private string _nomAffichage;

        // L'unité, si applicable (ex: "m", "m²", "kg/m³")
        [ObservableProperty]
        private string? _unite;

        // La catégorie pour le regroupement dans l'interface
        [ObservableProperty]
        private string? _categorie;
        public abstract object? ValeurObjet { get; set; }
        //public abstract object? Valeur { get; set; }


        // Constructeur
        protected Propriete(string nom, string nomAffichage)
        {
            _nom = nom;
            _nomAffichage = nomAffichage;
        }
    }
}
