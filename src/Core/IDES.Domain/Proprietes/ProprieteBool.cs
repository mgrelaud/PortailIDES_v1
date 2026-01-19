// Fichier : Models/Proprietes/ProprieteBool.cs
namespace IDES.Domain.Proprietes
{
    public partial class ProprieteBool : Propriete<bool>
    {
        public ProprieteBool(string nom, string nomAffichage)
            : base(nom, nomAffichage, false) { }
    }
}
