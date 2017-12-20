using System;
using System.Data;
using System.Data.SqlClient;

namespace CoreLibrary.Classes
{
    public class ExecuteScript
        :Base.IExecuteScript
    {
        /// <summary>
        /// Получаем имя ПК
        /// </summary>

        public string NamePc()
        {
            //Получаем имя ПК
            //TODO: перед релизной сборкой вернуть раскомментировать строку ниже
            string namePc = Environment.MachineName.ToLower();
            //string namePc = "R54-630024-N";

            //Получаем индекс
            string[] index = namePc.Split('-');
            string dataDaseName = "DB" + index[1];

            //Возвращаем имя ДБ
            return dataDaseName;
        }



        /// <summary>
        /// Execute script Export
        /// </summary>
        public void ScriptExport()
        {
            SqlConnection connection = new SqlConnection("Server = localhost; " + "Initial Catalog = " + NamePc() + ";" + "Integrated Security = SSPI");

            SqlCommand sqlCommand = new SqlCommand
            {
                CommandText = "use " + NamePc() + " exec ReplicaExport 0",
                CommandType = CommandType.Text,
                Connection = connection
            };
            try
            {
                connection.Open();
                //Logger.Log.Info("Успешно подключились к БД " + NamePc() + "для выполнения скрипта ReplicaExport");

                sqlCommand.CommandTimeout = 240;
                sqlCommand.ExecuteNonQuery();
                connection.Close();

                //Logger.Log.Info("Успешно выполнил скрипт ReplicaExport на БД " + NamePc() + "соединение закрыл");
            }
            catch (Exception exception)
            {
                //Logger.Log.Error(exception.Message);
            }
        }

        /// <summary>
        /// Execite script Import
        /// </summary>
        public void ScriptImport()
        {
            SqlConnection connection = new SqlConnection("Server = localhost; " + "Initial Catalog = " + NamePc() + ";" + "Integrated Security = SSPI");

            SqlCommand sqlCommand = new SqlCommand
            {
                CommandText = "use " + NamePc() + " exec ReplicaImport 0",
                CommandType = CommandType.Text,
                Connection = connection
            };
            try
            {
                connection.Open();
                Console.WriteLine();
                //Logger.Log.Info("Успешно подключились к БД " + NamePc() + "для выполнения скрипта ReplicaImport");
                sqlCommand.CommandTimeout = 240;
                sqlCommand.ExecuteNonQuery();

                //Logger.Log.Info("Успешно выполнил скрипт ReplicaImport на БД " + NamePc() + "соединение закрыл");
            }
            catch (Exception exception)
            {
                //Logger.Log.Error(exception.Message);
            }
        }
    }
}
