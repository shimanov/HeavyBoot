using System.ComponentModel;
using System.ServiceProcess;

namespace KeeperService
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public Installer()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "KeeperService";
            serviceInstaller.DisplayName = "KeeperService";
            serviceInstaller.Description = "Служба для автоматического восстановления работы транспорта.";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
