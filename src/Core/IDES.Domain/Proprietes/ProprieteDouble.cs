// Fichier : Models/Proprietes/ProprieteDouble.cs
namespace IDES.Domain.Proprietes
{
    public partial class ProprieteDouble : Propriete<double>
    {
        // Le constructeur passe simplement la valeur par défaut (0.0)
        public ProprieteDouble(string nom, string nomAffichage)
            : base(nom, nomAffichage, 0.000) { }
    }
}
