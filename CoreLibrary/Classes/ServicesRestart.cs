using System.ServiceProcess;

namespace CoreLibrary.Classes
{
    public class ServicesRestart
        : Base.IServicesRestart
    {
        /// <summary>
        /// Restart GMMQ service
        /// </summary>
        public void RestartGmmq()
        {
            ServiceController gmmq = new ServiceController("GMMQ");
            if (gmmq.Status == ServiceControllerStatus.Running)
            {
                gmmq.Stop();
                gmmq.WaitForStatus(ServiceControllerStatus.Stopped);

                //Logger.Log.Info("Останавливаем службу GMMQ");

                gmmq.Start();
                gmmq.WaitForStatus(ServiceControllerStatus.Running);

                //Logger.Log.Info("Запускаем службу GMMQ");
            }
            else
            {
                gmmq.Start();
                gmmq.WaitForStatus(ServiceControllerStatus.Running);

                //Logger.Log.Info("Служба GMMQ была остановлена. Запустил службу...");
            }
        }

        /// <summary>
        /// Restart GM_Scheduler process
        /// </summary>
        public void RestartScheduler()
        {
            ServiceController sheduler = new ServiceController("GM_SchedulerSvc");
            if (sheduler.Status == ServiceControllerStatus.Running)
            {
                sheduler.Stop();
                sheduler.WaitForStatus(ServiceControllerStatus.Stopped);

                //Logger.Log.Info("Останавливаем службу GM_SchedulerSvc");

                sheduler.Start();
                sheduler.WaitForStatus(ServiceControllerStatus.Running);

                //Logger.Log.Info("Запускаем службу GM_SchedulerSvc");
            }
            else
            {
                sheduler.Start();
                sheduler.WaitForStatus(ServiceControllerStatus.Running);

                //Logger.Log.Info("Служба GM_SchedulerSvc была остановлена. Запустил службу...");
            }
        }
    }
}
