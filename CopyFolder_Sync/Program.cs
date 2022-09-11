// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.IO;
class Program
{
    static void Main(string[] args)
    {
        string sourcePath = @"F:\task office\task2folder1";
        string destinationPath = @"F:\task office\task2folder2";
        var source = new DirectoryInfo(sourcePath);
        var destination = new DirectoryInfo(destinationPath);

        CopyFolderContents(sourcePath, destinationPath, "", true, true);
        DeleteAll(source, destination);
    }

    public static void CopyFolderContents(string sourceFolder, string destinationFolder, string mask, Boolean createFolders, Boolean recurseFolders)
    {

        try
        {

            var exDir = sourceFolder;
            var dir = new DirectoryInfo(exDir);
            var destDir = new DirectoryInfo(destinationFolder);

            SearchOption so = (recurseFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (string sourceFile in Directory.GetFiles(dir.ToString(), mask, so))
            {
                FileInfo srcFile = new FileInfo(sourceFile);
                string srcFileName = srcFile.Name;

                // Create a destination that matches the source structure
                FileInfo destFile = new FileInfo(destinationFolder + srcFile.FullName.Replace(sourceFolder, ""));

                if (!Directory.Exists(destFile.DirectoryName) && createFolders)
                {
                    Directory.CreateDirectory(destFile.DirectoryName);
                }

                //Check if src file was modified and modify the destination file
                if (srcFile.LastWriteTime > destFile.LastWriteTime || !destFile.Exists)
                {
                    File.Copy(srcFile.FullName, destFile.FullName, true);

                }
                while (srcFile.LastWriteTime > destFile.LastWriteTime || !destFile.Exists)
                {

                    int milliseconds = 100000;  //300000 milliseconds = 5 minutes
                    Thread.Sleep(milliseconds);
                    File.Copy(srcFile.FullName, destFile.FullName, true);
                }

            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
        }
    }

    private static void DeleteAll(DirectoryInfo source, DirectoryInfo target)
    {
        if (!source.Exists)
        {
            target.Delete(true);
            return;
        }

        // Delete each existing file in target directory not existing in the source directory.
        foreach (FileInfo fi in target.GetFiles())
        {
            var sourceFile = Path.Combine(source.FullName, fi.Name);
            if (!File.Exists(sourceFile)) //Source file doesn't exist, delete target file
            {
                fi.Delete();
            }
        }

        // Delete non existing files in each subdirectory using recursion.
        foreach (DirectoryInfo diTargetSubDir in target.GetDirectories())
        {
            DirectoryInfo nextSourceSubDir = new DirectoryInfo(Path.Combine(source.FullName, diTargetSubDir.Name));
            DeleteAll(nextSourceSubDir, diTargetSubDir);
        }
    }
}
