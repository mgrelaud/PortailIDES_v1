using System.Diagnostics;
using System.Runtime.InteropServices;
using PortailMetier.Domain.Interfaces;

namespace PortailMetier.Frontend.Services;

public class FileOperationsService : IFileOperationsService
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHELLEXECUTEINFO
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpVerb;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpFile;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpDirectory;
        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpClass;
        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

    private const uint SEE_MASK_INVOKEIDLIST = 0xC;

    public void OpenFile(string filePath)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FileOperations] Error opening file: {ex.Message}");
        }
    }

    public void ShowFileProperties(string filePath)
    {
        SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
        info.cbSize = Marshal.SizeOf(info);
        info.lpVerb = "properties";
        info.lpFile = filePath;
        info.nShow = 5; // SW_SHOW
        info.fMask = SEE_MASK_INVOKEIDLIST;
        ShellExecuteEx(ref info);
    }

    public async void CopyToClipboard(string filePath)
    {
        try 
        {
#if WINDOWS
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            
            var storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(filePath);
            dataPackage.SetStorageItems(new[] { storageFile });
            
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
#else
            await Microsoft.Maui.ApplicationModel.Clipboard.Default.SetTextAsync(filePath);
#endif
            Console.WriteLine($"[FileOperations] Copied file to clipboard: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FileOperations] Error copying file: {ex.Message}");
        }
    }

    public async void CutToClipboard(string filePath)
    {
        try 
        {
#if WINDOWS
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            
            var storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(filePath);
            dataPackage.SetStorageItems(new[] { storageFile });
            
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
#endif
            Console.WriteLine($"[FileOperations] Cut file to clipboard: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FileOperations] Error cutting file: {ex.Message}");
        }
    }

    public void OpenFolder(string filePath)
    {
        try
        {
            string argument = "/select, \"" + filePath + "\"";
            Process.Start("explorer.exe", argument);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FileOperations] Error opening folder: {ex.Message}");
        }
    }

    public void MergePdfs(List<string> filePaths)
    {
        // On pourrait appeler une ligne de commande si PDFCreator est présent
        // Mais pour l'instant on ouvre juste le premier
        if (filePaths.Any()) OpenFile(filePaths.First());
    }
}
