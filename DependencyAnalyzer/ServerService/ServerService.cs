using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using CodeAnalysis;

namespace HandCraftedService
{
    [ServiceBehavior]
    public class ServerService : IServerService
    {
        private ServiceHost host_;

        public ServiceHost host
        {
            get { return host_; }
            set { host_ = value; }
        }
        // Method for server's child thread to run to process messages.
        // It's virtual so you can derive from this service and define
        // some other server functionality.
        
        public void getTypeTables(RequestDetails requestDetails)
        {
            Executive exec = new Executive();
           
            ServerToMaster serverData = exec.getTypeTable(requestDetails);
            Sender sender = new Sender(DependencyConstants.localhostUrlPrefix + DependencyConstants.masterServerPortNo +
            "/" + DependencyConstants.masterServerPath);
            sender.actSenderForMasterServer(serverData);
        }

    }

}
