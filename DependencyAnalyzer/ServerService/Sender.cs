using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using HandCraftedService;
using System.Threading;
using CodeAnalysis;


namespace HandCraftedService
{
    public class Sender
    {
        private string _url;

        public string url
        {
            get { return _url; }
            set { _url = value; }
        }

        public Sender(string url)
        {
            this.url = url;
        }


        public ServiceHost CreateChannel<T, U>()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            Uri address = new Uri(this.url);
            Type service = typeof(T);
            ServiceHost host = new ServiceHost(service, address);
            host.AddServiceEndpoint(typeof(U), binding, address);
            return host;
        }

        

        public T CreateProxy<T>()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
           
            EndpointAddress address = new EndpointAddress(this.url);
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
            return factory.CreateChannel();
        }



        public T getSVC<T>()
        {
            T svc;
            int count = 0;
            while (true)
            {
                try
                {
                    svc = CreateProxy<T>();
                    break;
                }
                catch
                {
                    Console.Write("\n  connection to service failed {0} times - trying again", ++count);
                    Thread.Sleep(100);
                    continue;
                }
            }
            return svc;
        }

         void performMasterServerService(ServerToMaster serverToMaster)
        {
            IMasterServerService svc = getSVC<IMasterServerService>();            
            svc.resolveDependencyInTypeTables(serverToMaster);
        }

        public void actSenderForMasterServer(ServerToMaster serverToMaster)
        {
            Console.Write("\n  Starting Programmatic Basic Service- " + serverToMaster.requestDetails.serverName + " as Sender");            
            Console.Write("\n ============================================\n");
            performMasterServerService(serverToMaster);
        }
    }
}
