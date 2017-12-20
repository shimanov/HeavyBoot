using CoreLibrary.Classes;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace KeeperService
{
    public partial class KeeperService : ServiceBase
    {
        private System.Timers.Timer timers;
        public KeeperService()
        {
            InitializeComponent();
            CanStop = true;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            //timers = new System.Timers.Timer(30000D); //30 sec
            //timers.AutoReset = true;
            //timers.Elapsed += Timers_Elapsed;
            //timers.Start();

            Thread thread = new Thread(Govno);
        }

        //private void Timers_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    RunAsync().Wait();
        //}

        protected override void OnStop()
        {
            timers.Stop();
            timers = null;
        }

        static void Govno(object o)
        {
            while (true)
            {
                RunAsync().Wait();
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }

            static HttpClient client = new HttpClient();
            string pcName = Environment.MachineName.ToLower();

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

            //PUT
            static async Task<HBDataTable> UpdateData(HBDataTable dataTable)
            {
                string pcName = Environment.MachineName.ToLower();
                HttpResponseMessage response = await client.PutAsJsonAsync($"http://r54web03.main.russianpost.ru:52473/api/Govno/" + pcName, dataTable);
                response.EnsureSuccessStatusCode();


                dataTable = await response.Content.ReadAsAsync<HBDataTable>();
                return dataTable;
            }

            //Post
            static async Task<Uri> CreateDataAsync(HBDataTable dataTable)
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("http://r54web03.main.russianpost.ru:52473/api/Govno", dataTable);
                response.EnsureSuccessStatusCode();

                return response.Headers.Location;
            }

            //GET by pcname
            static async Task<HBDataTable> GetDatasync(string path)
            {
                HBDataTable data = null;
                ChangeDateTime dateTime = new ChangeDateTime();

                HttpResponseMessage message = await client.GetAsync(path);
                if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    data = await message.Content.ReadAsAsync<HBDataTable>();
                    if (data.IsChecked == true)
                    {
                        //тут запускаем keeper
                        Export();
                        Import();

                        //данные апдейтим на сервере
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

                        if (data.IsChecked == false)
                        {
                        //данные апдейтим на сервере
                        HBDataTable dataTable1 = new HBDataTable
                        {
                            Pcname = Environment.MachineName.ToLower(),
                            ClientTime = DateTime.Now,
                            DateServer = DateTime.Now,
                            ExportTime = dateTime.ChangeTimeExport(),
                            ImportTime = dateTime.ChangeTimeImport(),
                            IsChecked = false,
                            TimeOut = 15
                        };
                        await UpdateData(dataTable1);
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
                            TimeOut = 15
                        };
                        await CreateDataAsync(hBDataTable);
                    }

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
                        TimeOut = 15
                    };
                    await CreateDataAsync(hBDataTable);
                }
                return data;
            }

            static async Task RunAsync()
            {
                string pcName = Environment.MachineName.ToLower();
                client.BaseAddress = new Uri("http://r54web03.main.russianpost.ru:52473/api/govno");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HBDataTable dataTable = new HBDataTable();
                ChangeDateTime dateTime = new ChangeDateTime();

                //Get
                dataTable = await GetDatasync("http://r54web03.main.russianpost.ru:52473/api/govno/" + pcName);

                //Update
                dataTable.Pcname = pcName;
                dataTable.ClientTime = DateTime.Now;
                dataTable.DateServer = DateTime.Now;
                dataTable.ExportTime = dateTime.ChangeTimeExport();
                dataTable.ImportTime = dateTime.ChangeTimeImport();
                dataTable.IsChecked = false;
                dataTable.TimeOut = 15;
                await UpdateData(dataTable);
            }

        public class ServiceReference
        {
            public interface IService
            {
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //RunAsync().Wait();
        }
    }
}
