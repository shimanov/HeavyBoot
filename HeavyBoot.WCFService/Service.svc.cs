using System;
using System.Linq;
using System.ServiceModel;
using HeavyBoot.DAL.DatabaseConnection;
using HeavyBoot.DAL.Entitys;

namespace HeavyBoot.WCFService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Service : IService
    {
        public DateTime DataTable(string pcname, DateTime dateClient, DateTime exporTime, DateTime importTime, 
               byte timeOut, bool isCkecked)
        {
            //Соединение с БД
            var dbconnection = new Dbconnection();

            //Ищем запись в БД
            var dataTable = dbconnection.HbDataTables.Find(pcname);

            //Если запись уже существует, тогда обновлем запись pcname
            if (dataTable != null)
                try
                {
                    var data = dbconnection.HbDataTables
                        .Where(x => x.Pcname == pcname)
                        .Select(x => x).ToArray();
                    dataTable.ClientTime = dateClient;
                    dataTable.ExportTime = exporTime;
                    dataTable.ImportTime = importTime;
                    dataTable.IsChecked = isCkecked;
                    dataTable.TimeOut = timeOut;
                    dbconnection.SaveChanges();
                    return dateClient.AddMinutes(2);
                }
                catch (Exception e)
                {
                    return dateClient.AddMinutes(timeOut); 
                }

            //Если записи нет, тогда добавляем ее
            try
            {
                dbconnection.HbDataTables.Add(new HBDataTable
                {
                    Pcname = pcname,
                    ClientTime = dateClient,
                    DateServer = DateTime.Now,
                    ExportTime = exporTime,
                    ImportTime = importTime,
                    TimeOut = timeOut,
                    IsChecked = false

                });
                dbconnection.SaveChanges();
                return DateTime.Now;
            }
            catch (Exception e)
            {
                return DateTime.Now;
            }
        }
    }
}
