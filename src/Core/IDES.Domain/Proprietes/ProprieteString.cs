// Fichier : Models/Proprietes/ProprieteString.cs
namespace IDES.Domain.Proprietes
{
    public partial class ProprieteString : Propriete<string>
    {
        // Le constructeur passe la valeur par défaut (une chaîne vide)
        public ProprieteString(string nom, string nomAffichage)
            : base(nom, nomAffichage, string.Empty) { }
    }
}

