using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using HandCraftedService;
using System.Threading;


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
            binding.MaxReceivedMessageSize = 10485760;
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

        void performClientService(string clientName,string typeXML, string packageXML)
        {
            IClientService svc = getSVC<IClientService>();
            svc.workOnXMLData(clientName, typeXML, packageXML);   

        }

       

        public void actSender(string name, string clientName, string typeXML, string packageXML)
        {
            Console.Write("\n  Starting Programmatic Basic Service- " + name + " as Sender");
            Console.Write("\n ============================================\n");
            performClientService(clientName,typeXML, packageXML);
        }

       

    }
}
