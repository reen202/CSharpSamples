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

         T CreateProxy<T>()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.SendTimeout = new TimeSpan(0, 30, 30);
            EndpointAddress address = new EndpointAddress(this.url);
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
            return factory.CreateChannel();
        }



         T getSVC<T>()
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

         void performServerService( RequestDetails reqDetails)
        {
            IServerService svc = getSVC<IServerService>();
            //String[] files = { "abc.cs", "def.cs", "xyz.cs" };
            svc.getTypeTables(reqDetails);
         }
        
        public  void actSenderForSlaveServer(RequestDetails reqDetails)
        {
            Console.Write("\n  Starting Programmatic Basic Service- "+reqDetails.clientName+" as Sender");
            Console.Write("\n ============================================\n");
            performServerService(reqDetails);          

        }

       

       
    }
}
