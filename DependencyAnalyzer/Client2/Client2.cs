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

namespace ServiceClient
{
    class Client2
    {
        static T CreateProxy<T>(string url)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress address = new EndpointAddress(url);
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
            return factory.CreateChannel();
        }



        static T getSVC<T>(String url)
        {
            T svc;
            int count = 0;
            while (true)
            {
                try
                {

                    svc = CreateProxy<T>(url);


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




        static void performServerService(string url)
        {
            IServerService svc = getSVC<IServerService>(url);
            String[] files = { "abc.cs", "def.cs", "xyz.cs" };
            svc.generateTypeTables(files);
            Console.Write("\n 1st Message recieved from Server Service: {0}\n\n", svc.getTypeTables());
            Console.Write("\n 2nd Message recieved from Server Service: {0}\n\n", svc.sendMasterServerForDependency());
            Console.ReadKey();
        }

       


        static void Main(string[] args)
        {
            Console.Write("\n  Starting Programmatic Basic Service Client2");
            Console.Write("\n ============================================\n");



            performServerService("http://localhost:8081/ServerService");
            performMasterServerService("http://localhost:8082/MasterServerService");

        }
    }
}
