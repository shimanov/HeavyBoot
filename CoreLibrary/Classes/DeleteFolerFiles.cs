using System;
using System.IO;

namespace CoreLibrary.Classes
{
    public class DeleteFolerFiles
        :Base.IDeleteFolderFiles
    {
        /// <summary>
        /// Очистка каталога Export
        /// </summary>
        public void DeleteFolder()
        {
            string deleteExportPath = @"C:\GMMQ\Export";
            DeleteFolder(deleteExportPath);
            Console.WriteLine(DateTime.Now + " Каталог Export очищен");
        }

        /// <summary>
        /// Очистка каталога Import
        /// </summary>
        public void CleanFolderImport()
        {
            string deleteExportPath = @"C:\GMMQ\Import";
            DeleteFolder(deleteExportPath);
            Console.WriteLine(DateTime.Now + " Каталог Import очищен");
        }

        static void DeleteFolder(string folder)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(folder);

                //Создаем массив дочерних вложенных каталогов folder
                DirectoryInfo[] directoryInfos = directory.GetDirectories();
                FileInfo[] file = directory.GetFiles();
                foreach (FileInfo f in file)
                {
                    f.Delete();
                }

                foreach (DirectoryInfo d in directoryInfos)
                {
                    DeleteFolder(d.FullName);

                    if (d.GetDirectories().Length == 0 && d.GetFiles().Length == 0)
                    {
                        d.Delete();
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Log.Error(e.ToString());
            }
        }
    }
}
