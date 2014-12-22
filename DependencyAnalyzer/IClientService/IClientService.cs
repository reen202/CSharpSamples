using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using CodeAnalysis;

namespace HandCraftedService
{
    [ServiceContract(Namespace = "HandCraftedService")]
    public interface IClientService
    {
      
        [OperationContract]
        void workOnXMLData(String clientName, String typeXML, String packageXML);
       
    }

}

