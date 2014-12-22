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
    public class MasterServerService : IMasterServerService
    {

        public void resolveDependencyInTypeTables(ServerToMaster serverToMaster)
        {
            Console.WriteLine("MasterServerService : Received the Server To Master Object from "+serverToMaster.requestDetails.serverName);
            Console.WriteLine("Received object Details: ");
            if (serverToMaster.relTable != null) { 
                Console.WriteLine("RelTable size"+serverToMaster.relTable.Count);
            }
            if (serverToMaster.typeTable != null && serverToMaster.typeTable.types != null) { 
                Console.WriteLine("Type Table size" + serverToMaster.typeTable.types.Count);
            }
            PackageAnalyzer.masterDataQueue.enQ(serverToMaster);           
            String[] xmlData = PackageAnalyzer.processMasterData();
            if (xmlData != null)
            {
                Console.WriteLine("Sending response for " + serverToMaster.requestDetails.clientName + "from MasterServer");
                Sender sender1 = new Sender(serverToMaster.requestDetails.clientUrl);
                sender1.actSender(DependencyConstants.masterServer1Name,serverToMaster.requestDetails.clientName,
                    xmlData[0], xmlData[1]);
            }
            
        }

    }

}
