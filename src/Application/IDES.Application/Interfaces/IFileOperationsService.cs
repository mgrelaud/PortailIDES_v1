namespace IDES.Application.Interfaces;

public interface IFileOperationsService
{
    /// <summary>
    /// Ouvre le fichier avec l'application par défaut
    /// </summary>
    void OpenFile(string filePath);

    /// <summary>
    /// Affiche les propriétés Windows du fichier
    /// </summary>
    void ShowFileProperties(string filePath);

    /// <summary>
    /// Copie le fichier dans le presse-papier
    /// </summary>
    void CopyToClipboard(string filePath);

    /// <summary>
    /// Coupe le fichier (prépare pour le déplacement)
    /// </summary>
    void CutToClipboard(string filePath);

    /// <summary>
    /// Ouvre le dossier contenant le fichier
    /// </summary>
    void OpenFolder(string filePath);
    
    /// <summary>
    /// Fusionne des PDF (nécessite une logique spécifique ou appel externe)
    /// </summary>
    void MergePdfs(List<string> filePaths);
}
