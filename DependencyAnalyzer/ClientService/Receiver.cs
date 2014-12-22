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

        public Receiver()
        {

        }

        public Receiver(string url)
        {
            this.url = url;
        }


        public ServiceHost CreateChannel<T, U>()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            Uri address = new Uri(this.url);
            Type service = typeof(T);
            ServiceHost host = new ServiceHost(service, address);
            binding.SendTimeout = new TimeSpan(0, 30, 30);

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
                host = CreateChannel<ClientService, IClientService>();
                host.Open();

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

        public void actMultipleRecievers(List<string> urls)
        {
            Console.Write("\n  Communication Server Starting up");
            Console.Write("\n ==================================\n");

            try
            {
                ClientService service = new ClientService();

                // - We're using WSHttpBinding and NetTcpBinding so digital certificates
                //   are required.
                // - Both these bindings support ordered delivery of messages by default.               
                List<Uri> uriList = new List<Uri>();
                List<BasicHttpBinding> bindingList = new List<BasicHttpBinding>();
                foreach (string url in urls)
                {
                    Uri address = new Uri(url);
                    uriList.Add(address);
                    BasicHttpBinding binding = new BasicHttpBinding();
                    binding.SendTimeout = new TimeSpan(0, 30, 30);
                    bindingList.Add(binding);
                }


                using (service.host = new ServiceHost(typeof(ClientService), uriList[0]))
                {
                    int i = 0;
                    foreach (Uri address in uriList)
                    {
                        service.host.AddServiceEndpoint(typeof(IClientService), bindingList[i], address);
                        i++;
                    }

                    service.host.Open();
                    Console.WriteLine();
                    Console.Write("\n\n All serverPorts started  Press <ENTER> to terminate service.\n\n");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}\n\n", ex.Message);
            }

        }

    }
}
