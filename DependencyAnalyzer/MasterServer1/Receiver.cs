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
public class Receiver
    {
        private string _url;


        public string url
        {
            get { return _url; }
            set { _url = value; }
        }

        Receiver()
        {

        }

        public Receiver(string url)
        {
            this.url = url;
        }


        public ServiceHost CreateChannel<T, U>()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 10485760;
            Uri address = new Uri(this.url);
            Type service = typeof(T);
            ServiceHost host = new ServiceHost(service, address);
            host.AddServiceEndpoint(typeof(U), binding, address);
            return host;
        }

        public void actReceiver(string name)
        {
            ServiceHost host = null;
            try
            {
                Console.Write("\n  Starting Programmatic Basic Service - " + name + " as Receiver");
                Console.Write("\n =====================================\n");
                host = CreateChannel<MasterServerService, IMasterServerService>();
                host.Open();
                Console.ReadKey();               
            }
            catch (Exception ex)
            {
                Console.Write("\n\n  {0}\n\n", ex.Message);
            }
            finally
            {
                // host.Close();
            }

        }

    }
}
