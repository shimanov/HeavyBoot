using CoreLibrary.Classes;
using HeavyBoot.DAL.Entitys;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestConnection
{
    class Program
    {
        static HttpClient client = new HttpClient();
        ChangeDateTime dateTime = new ChangeDateTime();


        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            string pcName = Environment.MachineName.ToLower();
            client.BaseAddress = new Uri("http://r54web03.main.russianpost.ru:52473/api/govno");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            ChangeDateTime dateTime = new ChangeDateTime();

            try
            {
                HBDataTable dataTable = new HBDataTable();

                //Get
                dataTable = await GetDatasync("http://r54web03.main.russianpost.ru:52473/api/govno/" + pcName);
            }
            catch (Exception e)
            {
            }

            Console.ReadLine();
        }

        //Get
        /// <summary>
        /// GET запрос GetByPcname ({pcname})
        /// Логика работы:
        /// 1. Делаем GET запрос с именеим ПК на сервер
        /// 2. Если статут ответа 200 ОК, тогда собираем данные и отправляем на сервер
        /// 3. Если статус ответа отличный от 200 ОК, значит клиент не зарегистрирован, собираем данные и отправляем на сервер
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static async Task<HBDataTable> GetDatasync(string path)
        {
            HBDataTable data = null;
            HttpResponseMessage message = await client.GetAsync(path);
            ChangeDateTime dateTime = new ChangeDateTime();
            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                data = await message.Content.ReadAsAsync<HBDataTable>();
                if (data.IsChecked == true)
                {
                    //тут запускаем keeper, и данные апдейтим на сервере
                    Export();
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                    Import();

                    //Update data
                    HBDataTable dataTable = new HBDataTable
                    {
                        Pcname = Environment.MachineName.ToLower(),
                        ClientTime = DateTime.Now,
                        DateServer = DateTime.Now,
                        ExportTime = dateTime.ChangeTimeExport(),
                        ImportTime = dateTime.ChangeTimeImport(),
                        IsChecked = false,
                        TimeOut = 15
                    };
                    await UpdateData(dataTable);
                }
                if (data.IsChecked == false)
                {
                    HBDataTable dataTable = new HBDataTable
                    {
                        Pcname = Environment.MachineName.ToLower(),
                        ClientTime = DateTime.Now,
                        DateServer = DateTime.Now,
                        ExportTime = dateTime.ChangeTimeExport(),
                        ImportTime = dateTime.ChangeTimeImport(),
                        IsChecked = false,
                        TimeOut = 15
                    };
                    await UpdateData(dataTable);
                }

                return data;
            }
            else
            {
                //Create
                HBDataTable hBDataTable = new HBDataTable
                {
                    Pcname = Environment.MachineName.ToLower(),
                    ClientTime = DateTime.Now,
                    DateServer = DateTime.Now,
                    ExportTime = dateTime.ChangeTimeExport(),
                    ImportTime = dateTime.ChangeTimeImport(),
                    IsChecked = false,
                    TimeOut = 10
                };
                await CreateDataAsync(hBDataTable);
            }
            return data;
        }

        //Post
        static async Task<Uri> CreateDataAsync(HBDataTable dataTable)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("http://r54web03.main.russianpost.ru:52473/api/Govno", dataTable);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        //PATCH
        static async Task<HBDataTable> UpdateData(HBDataTable dataTable)
        {
            string pcName = Environment.MachineName.ToLower();
            HttpResponseMessage response = await client.PutAsJsonAsync($"http://r54web03.main.russianpost.ru:52473/api/Govno/" + pcName, dataTable);
            response.EnsureSuccessStatusCode();

            
            dataTable = await response.Content.ReadAsAsync<HBDataTable>();
            return dataTable;
        }

        #region Export

        static void Export()
        {
            /********************************************************************************
             * Если в каталоге Export есть файл gmmq.packedge.end,
             * тогда перезапускаем службы
             * если файл gmmq.packedge.end отсутсвует, очищаем каталог 
             * и выполняем скрипты на выгрузку реплики 
             *******************************************************************************/

            string pathFileExportG = "C:\\GMMQ\\Export\\gmmq.package.end";

            if (File.Exists(pathFileExportG))
            {
                //TODO: перед релизной сборкой снять комментарий
                ServicesRestart restart = new ServicesRestart();
                restart.RestartGmmq();
                restart.RestartScheduler();
            }
            else
            {
                //Чистм катаоги
                var delete = new DeleteFolerFiles();
                delete.DeleteFolder();

                //Делаем экспорт
                var script = new ExecuteScript();
                script.ScriptExport();

                //Logger.Log.Info("Ждем 5 минут");


                //Перезапускаем службы
                ServicesRestart services = new ServicesRestart();
                services.RestartGmmq();
                services.RestartScheduler();
            }
        }

        #endregion

        #region Import

        static void Import()
        {
            /********************************************************************************
            * Если в каталоге Import есть файл gmmq.packedge.end,
            * тогда выполняем скрипт на всасывание реплики
            * если файл gmmq.packedge.end отсутсвует, очищаем каталог 
            *******************************************************************************/

            string pathFileImport = "C:\\GMMQ\\Import\\gmmq.package.end";

            if (File.Exists(pathFileImport))
            {
                //Всасываем реплику
                var script = new ExecuteScript();
                script.ScriptImport();
            }
            else
            {
                //Чистим каталог
                var delete = new DeleteFolerFiles();
                delete.CleanFolderImport();
            }
        }

        #endregion
    }
}
