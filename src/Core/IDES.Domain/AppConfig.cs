using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;
namespace IDES.Domain
{
    // La classe doit être 'partial' pour que les "Source Generators" puissent ajouter du code.
    public partial class AppConfig : ObservableObject
    {
        // --- Couleurs ---
        // Le champ est privé, en camelCase, avec un '_'.
        // L'attribut [ObservableProperty] génère la propriété publique 'CouleurJetons' en PascalCase.
        [ObservableProperty]
        private string _couleurJetons = "#FF6200EE";

        [ObservableProperty]
        private string _couleurTitre = "#E3F2FD";

        [ObservableProperty]
        private string _couleurSousTitre = "#E3F2FD";

        [ObservableProperty]
        private string _couleurSousSousTitre = "#E3F2FD";

        // --- Format de numérotation - Titre ---
        // L'attribut [NotifyPropertyChangedFor] mettra à jour les propriétés d'aperçu
        // automatiquement chaque fois que cette valeur change.
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ApercuTitre))]
        [NotifyPropertyChangedFor(nameof(ApercuSousTitre))]
        [NotifyPropertyChangedFor(nameof(ApercuSousSousTitre))]
        private string _typeTitre = "A";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ApercuTitre))]
        private string _terminaisonTitre = "/";

        // --- Format de numérotation - Sous-titre ---
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ApercuSousTitre))]
        [NotifyPropertyChangedFor(nameof(ApercuSousSousTitre))]
        private string _typeSousTitre = "1";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ApercuSousTitre))]
        [NotifyPropertyChangedFor(nameof(ApercuSousSousTitre))]
        private string _separateurSousTitre = ".";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ApercuSousTitre))]
        private string _terminaisonSousTitre = "/";

        // --- Format de numérotation - Sous-sous-titre ---
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ApercuSousSousTitre))]
        private string _typeSousSousTitre = "a";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ApercuSousSousTitre))]
        private string _separateurSousSousTitre = ".";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ApercuSousSousTitre))]
        private string _terminaisonSousSousTitre = "/";

        // --- Polices ---
        // On stocke le nom de la police (string)
        [ObservableProperty]
        private string _fontFamilyTitre = "Segoe UI";

        [ObservableProperty]
        private int _fontSizeTitre = 14;

        [ObservableProperty]
        private string _fontWeightTitre = "Bold";

        [ObservableProperty]
        private string _fontFamilySousTitre = "Segoe UI";

        [ObservableProperty]
        private int _fontSizeSousTitre = 14;

        [ObservableProperty]
        private string _fontWeightSousTitre = "Bold";

        [ObservableProperty]
        private string _fontFamilySousSousTitre = "Segoe UI";

        [ObservableProperty]
        private int _fontSizeSousSousTitre = 14;

        [ObservableProperty]
        private string _fontWeightSousSousTitre = "Normal";


        // --- Propriétés en lecture seule pour les aperçus ---
        // Ces propriétés n'ont pas de champ privé. Elles calculent leur valeur à la volée.
        // Elles sont mises à jour grâce aux attributs [NotifyPropertyChangedFor] ci-dessus.
        [JsonIgnore]
        public string ApercuTitre => $"{TypeTitre}{TerminaisonTitre}";
        [JsonIgnore]
        public string ApercuSousTitre => $"{TypeTitre}{SeparateurSousTitre}{TypeSousTitre}{TerminaisonSousTitre}";
        [JsonIgnore]
        public string ApercuSousSousTitre => $"{TypeTitre}{SeparateurSousTitre}{TypeSousTitre}{SeparateurSousSousTitre}{TypeSousSousTitre}{TerminaisonSousSousTitre}";
    }
}

//using CommunityToolkit.Mvvm.ComponentModel;
//using QuantitatifBeton.ViewModels;
//using System.Windows.Media;

//namespace IDES.Domain
//{
//    public partial class AppConfig : ObservableObject
//    {
//        // Utiliser [ObservableProperty] pour toutes les propriétés de configuration
//        [ObservableProperty]
//        // Couleurs
//        private string _couleurJetons = "#FF6200EE";
//        public string CouleurJetons
//        {
//            get => _couleurJetons;
//            set { _couleurJetons = value; OnPropertyChanged(); }
//        }

//        private string _couleurTitre = "#E3F2FD";
//        public string CouleurTitre
//        {
//            get => _couleurTitre;
//            set { _couleurTitre = value; OnPropertyChanged(); }
//        }

//        private string _couleurSousTitre = "#E3F2FD";
//        public string CouleurSousTitre
//        {
//            get => _couleurSousTitre;
//            set { _couleurSousTitre = value; OnPropertyChanged(); }
//        }

//        private string _couleurSousSousTitre = "#E3F2FD";
//        public string CouleurSousSousTitre
//        {
//            get => _couleurSousSousTitre;
//            set { _couleurSousSousTitre = value; OnPropertyChanged(); }
//        }

//        // Format de numérotation - Titre
//        private string _typeTitre = "A"; // "A", "a", "I", "i", "1"
//        public string TypeTitre
//        {
//            get => _typeTitre;
//            set { _typeTitre = value; OnPropertyChanged(); }
//        }

//        private string _terminaisonTitre = "/"; // "/", ")", ""
//        public string TerminaisonTitre
//        {
//            get => _terminaisonTitre;
//            set { _terminaisonTitre = value; OnPropertyChanged(); }
//        }

//        // Format de numérotation - Sous-titre
//        private string _typeSousTitre = "1"; // "A", "a", "I", "i", "1"
//        public string TypeSousTitre
//        {
//            get => _typeSousTitre;
//            set { _typeSousTitre = value; OnPropertyChanged(); }
//        }

//        private string _separateurSousTitre = "."; // ".", "-", ""
//        public string SeparateurSousTitre
//        {
//            get => _separateurSousTitre;
//            set { _separateurSousTitre = value; OnPropertyChanged(); }
//        }

//        private string _terminaisonSousTitre = "/";
//        public string TerminaisonSousTitre
//        {
//            get => _terminaisonSousTitre;
//            set { _terminaisonSousTitre = value; OnPropertyChanged(); }
//        }

//        // Format de numérotation - Sous-sous-titre
//        private string _typeSousSousTitre = "a"; // "A", "a", "I", "i", "1"
//        public string TypeSousSousTitre
//        {
//            get => _typeSousSousTitre;
//            set { _typeSousSousTitre = value; OnPropertyChanged(); }
//        }

//        private string _separateurSousSousTitre = ".";
//        public string SeparateurSousSousTitre
//        {
//            get => _separateurSousSousTitre;
//            set { _separateurSousSousTitre = value; OnPropertyChanged(); }
//        }

//        private string _terminaisonSousSousTitre = "/";
//        public string TerminaisonSousSousTitre
//        {
//            get => _terminaisonSousSousTitre;
//            set { _terminaisonSousSousTitre = value; OnPropertyChanged(); }
//        }
//        public string FontFamilyTitre { get; set; } = "Segoe UI";
//        public int FontSizeTitre { get; set; } = 14;
//        public string FontWeightTitre { get; set; } = "Bold";

//        public string FontFamilySousTitre { get; set; } = "Segoe UI";
//        public int FontSizeSousTitre { get; set; } = 14;
//        public string FontWeightSousTitre { get; set; } = "Bold";

//        public string FontFamilySousSousTitre { get; set; } = "Segoe UI";
//        public int FontSizeSousSousTitre { get; set; } = 14;
//        public string FontWeightSousSousTitre { get; set; } = "Normal";
//    }
//}
