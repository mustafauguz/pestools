using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MustafaUğuz.Utility.System.IO
{
    public static class Extensions
    {
        public static bool IsEmpty(this DirectoryInfo directory)
        {
            bool result = true;
            if (directory.Exists)
            {
                result = directory.GetFiles("*", SearchOption.TopDirectoryOnly).Length == 0;

                if (result)
                    result = directory.GetDirectories("*", SearchOption.TopDirectoryOnly).Length == 0;

                return result;
            }

            else
                return true;
        }

        public static void Remove(this DirectoryInfo directory)
        {
            if (directory.Exists)
            {
                var files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                    file.Remove();

                var directories = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);

                foreach (var dir in directories)
                    dir.Remove();

                directory.Delete();
            }
        }

        public static void Move(this DirectoryInfo directory, string destFolderPath)
        {
            directory.Move(destFolderPath, false);
        }

        public static void Move(this DirectoryInfo directory, string destFolderPath, bool removeDestFolder)
        {
            FileSystemInfo[] files, folders;
            var destDirectory = new DirectoryInfo(destFolderPath);

            if (removeDestFolder)
                destDirectory.Remove();

            files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
            folders = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
                ((FileInfo)file).Move(Path.Combine(destFolderPath, file.Name));

            foreach (var folder in folders)
                ((DirectoryInfo)folder).Move(Path.Combine(destFolderPath, folder.Name));

            if (directory.IsEmpty())
                directory.Remove();
        }

        public static void Copy(this DirectoryInfo directory, string destFolderPath)
        {
            directory.Copy(destFolderPath, false);
        }

        public static void Copy(this DirectoryInfo directory, string destFolderPath, bool removeDestFolder)
        {
            FileSystemInfo[] files, folders;
            var destDirectory = new DirectoryInfo(destFolderPath);

            if (removeDestFolder)
                destDirectory.Remove();

            files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
            folders = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
                ((FileInfo)file).Copy(Path.Combine(destFolderPath, file.Name));

            foreach (var folder in folders)
                ((DirectoryInfo)folder).Copy(Path.Combine(destFolderPath, folder.Name));
        }

        public static void Remove(this FileInfo file)
        {
            if (file.Exists)
                file.Delete();
        }

        public static void Move(this FileInfo file, string destFilePath)
        {
            new FileInfo(destFilePath).Remove();
            file.MoveTo(destFilePath);
        }

        public static void Copy(this FileInfo file, string destFilePath)
        {
            new FileInfo(destFilePath).Remove();
            file.CopyTo(destFilePath);
        }
    }
}
