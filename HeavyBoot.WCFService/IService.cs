using System;
using System.ServiceModel;

namespace HeavyBoot.WCFService
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        DateTime DataTable(string pcname, DateTime dateClient, DateTime exporTime, DateTime importTime, 
                           byte timeOut, bool isCkecked);
    }
}
