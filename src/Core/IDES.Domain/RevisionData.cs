// 1. AJOUTER le using du Community Toolkit
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace IDES.Domain
{
    // 2. AJOUTER 'partial' et s'assurer que la classe hérite du bon 'ObservableObject'
    public partial class RevisionData : ObservableObject
    {
        // 3. TRANSFORMER les propriétés en champs privés avec l'attribut [ObservableProperty]

        [ObservableProperty]
        private DateTime _date = DateTime.Now;

        [ObservableProperty]
        private string _indice = string.Empty;

        [ObservableProperty]
        private string _objet = string.Empty;

        [ObservableProperty]
        private string _par = string.Empty;
    }
}
