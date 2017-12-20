using System;
using System.IO;

namespace CoreLibrary.Classes
{
    public class ChangeDateTime
        :Base.IChangeDateTime
    {
        /// <summary>
        /// Возвращает дату и время изменения каталога export
        /// </summary>
        /// <returns>DateTime export</returns>
        public DateTime ChangeTimeExport()
        {
            DateTime export = File.GetLastWriteTime(@"C:\GMMQ\Export");
            return export;
        }

        /// <summary>
        /// Возвращает дату и время изменения каталога import
        /// </summary>
        /// <returns>DateTime export</returns>
        public DateTime ChangeTimeImport()
        {
            DateTime import = File.GetLastWriteTime(@"C:\GMMQ\Import");
            return import;
        }
    }
}
