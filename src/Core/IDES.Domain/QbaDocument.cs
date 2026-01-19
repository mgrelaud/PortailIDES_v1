using System;
using System.Collections.ObjectModel;

namespace IDES.Domain
{
    /// <summary>
    /// Document principal du métré quantitatif béton.
    /// Contient le cartouche, les révisions, le métré et la configuration.
    /// </summary>
    public class QbaDocument
    {
        /// <summary>
        /// Données du cartouche (titre, client, date, etc.)
        /// </summary>
        public CartoucheData Cartouche { get; set; } = new();

        /// <summary>
        /// Historique des révisions du document
        /// </summary>
        public ObservableCollection<RevisionData> Revisions { get; set; } = new();

        /// <summary>
        /// Liste de tous les éléments du métré (ElementDynamique et TitreElement)
        /// </summary>
        public ObservableCollection<object> Metre { get; set; } = new();

        /// <summary>
        /// Configuration du document
        /// </summary>
        public AppConfig Configuration { get; set; } = new();

        /// <summary>
        /// Chemin du fichier du document
        /// </summary>
        public string CheminFichier { get; set; } = string.Empty;

        /// <summary>
        /// Date de création du document
        /// </summary>
        public DateTime DateCreation { get; set; } = DateTime.Now;

        /// <summary>
        /// Date de dernière modification du document
        /// </summary>
        public DateTime DerniereModification { get; set; } = DateTime.Now;
    }
}
