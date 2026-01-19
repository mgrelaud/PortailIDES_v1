using System.Collections.ObjectModel;

namespace IDES.Domain
{
    public class NavigationNode
    {
        public string Numero { get; set; } = string.Empty;
        public string Texte { get; set; } = string.Empty;
        public TitreElement? TitreReference { get; set; }
        public ObservableCollection<NavigationNode> Children { get; set; } = new();
    }
}
