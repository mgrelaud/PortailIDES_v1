using CommunityToolkit.Mvvm.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IDES.Domain
{
    /// <summary>
    /// Élément représentant un titre ou sous-titre dans le métré.
    /// Les titres ne participent pas aux calculs (volume béton, acier, etc.)
    /// </summary>
    public partial class TitreElement : ObservableObject
    {
        /// <summary>
        /// Type de titre : Titre principal, Sous-titre, ou Sous-sous-titre
        /// </summary>
        [ObservableProperty]
        private TypeTitre _typeTitre = TypeTitre.Titre;

        /// <summary>
        /// Texte du titre (ex: "BATIMENT C", "FONDATIONS")
        /// </summary>
        [ObservableProperty]
        private string _texte = string.Empty;

        /// <summary>
        /// Numéro du titre (ex: "3/", "3.A/", "Rad1")
        /// </summary>
        [ObservableProperty]
        private string _numero = string.Empty;

        /// <summary>
        /// Désignation du titre (combinaison du numéro et du texte)
        /// </summary>
        public string Designation => $"{Numero} {Texte}".Trim();
    }

    /// <summary>
    /// Énumération des types de titres dans le métré
    /// </summary>
    public enum TypeTitre
    {
        /// <summary>Titre principal (ex: "3/ BATIMENT C")</summary>
        Titre,

        /// <summary>Sous-titre (ex: "3.A/ FONDATIONS")</summary>
        SousTitre,

        /// <summary>Sous-sous-titre (ex: "Rad1")</summary>
        SousSousTitre
    }
}
