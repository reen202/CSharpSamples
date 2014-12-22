/////////////////////////////////////////////////////////////////////////
// ProgClient.cs - Service Client for Programmatic BasicService demo   //
//                                                                     //
//   Uses BasicHttpBinding                                             //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
// - Started with C# Console Application Project
// - Made reference to .Net System.ServiceModel
// - Added using System.ServiceModel
// - Added code to create communication channel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HandCraftedService;
using System.Threading;
using CodeAnalysis;


namespace HandCraftedService
{
    public class Client1
    {
        public static string getMessage()
        {
            return "I am happy";
        }

        public static RequestDetails constructRequest1()
        {

            RequestDetails reqDetails = new RequestDetails();
            reqDetails.clientName = "Client1";
            reqDetails.clientUrl = "http://localhost:8081/ClientService";
            reqDetails.serverList = new List<string>();
            reqDetails.serverList.Add("Server1");
            reqDetails.serverList.Add("Server2");
            String[] args = { "../../../TestProject/Parser", "\\S", "\\X", "\\R", "*.cs" };
            reqDetails.args = args;
            reqDetails.serverName = DependencyConstants.slaveServer1Name;

            return reqDetails;
        }

        public static RequestDetails constructRequest2()
        {

            RequestDetails reqDetails = new RequestDetails();
            reqDetails.clientName = "Client1";
            reqDetails.clientUrl = "http://localhost:8081/ClientService";
            reqDetails.serverList = new List<string>();
            reqDetails.receivedSrvList.Add("Server1");
            reqDetails.serverList.Add("Server2");
            String[] args = { "../../../TestProject/Analyzer", "\\S", "\\X", "\\R", "*.cs" };
            reqDetails.args = args;
            reqDetails.serverName = DependencyConstants.slaveServer2Name;

            return reqDetails;
        }

        public static void queryServers(Dictionary<String, RequestDetails> clientData)
        {
            List<string> serverList = new List<string>();
            int numberOfServers = serverList.Count;
            int i = 0;
            Console.WriteLine("Number of servers : " + numberOfServers);


            List<string> urlList = new List<String>();
            Console.WriteLine("The Client urls are : ");
            foreach (var data in clientData)
            {
                string serverName = data.Key.ToString();
                RequestDetails req = data.Value;
                String url = req.clientUrl;
                urlList.Add(url);
                Console.WriteLine(url);
            }
            /*Receiver receiver1 = new Receiver();
            receiver1.actMultipleRecievers(urlList);
             */

            foreach (var data in clientData)
            {
                string serverName = data.Key.ToString();
                RequestDetails req = data.Value;

                /*Receiver receiver1 = new Receiver(req.clientUrl);
                receiver1.actReceiver(req.clientName);

                Console.WriteLine(req.clientUrl);*/
                string portNo = DependencyConstants.slaveServer1PortNo;
                if (req.serverName.Equals(DependencyConstants.slaveServer1Name))
                {
                    portNo = DependencyConstants.slaveServer1PortNo;
                }
                else
                {
                    portNo = DependencyConstants.salveServer2PortNo;
                }
                Sender sender1 = new Sender(DependencyConstants.localhostUrlPrefix + portNo +
            "/" + DependencyConstants.slaveServerPath);

                sender1.actSenderForSlaveServer(req);
                i++;
            }

        }

        #if(CLIENT)
        public static void Main(string[] args)
        {
            List<string> urlList = new List<String>();
            int startPort = 8081;
            for (startPort = 8081; startPort < 8089; startPort++)
            {
                urlList.Add(DependencyConstants.localhostUrlPrefix + startPort +
                "/" + DependencyConstants.clientPath);
            }
            Receiver receiver1 = new Receiver();
            receiver1.actMultipleRecievers(urlList);

            Console.ReadKey();
        }
        #endif
    }
        
}
