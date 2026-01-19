// 1. AJOUTER le using nécessaire
using CommunityToolkit.Mvvm.ComponentModel;

namespace IDES.Domain
{
    // 2. FAIRE HÉRITER de ObservableObject et AJOUTER 'partial'
    public partial class CartoucheData : ObservableObject
    {
        // 3. TRANSFORMER les propriétés en champs privés avec [ObservableProperty]
        [ObservableProperty]
        private string _titreDossier = string.Empty;

        [ObservableProperty]
        private string _soustitre = string.Empty;

        [ObservableProperty]
        private string _ville = string.Empty;

        [ObservableProperty]
        private string _numDossier = string.Empty;

        [ObservableProperty]
        private string _pilote = string.Empty;

        [ObservableProperty]
        private string _nomDossierRep = string.Empty;
    }
}
