using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using HandCraftedService;
using System.Diagnostics;
using CodeAnalysis;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using FinalOutput;

namespace HandCraftedService
{
    [ServiceBehavior]
    public class ClientService : IClientService
    {
        public ServiceHost host = null;

        public void workOnXMLData(String clientName, String typeXML, String packageXML)
        {
            Console.WriteLine("=========================================================");
            Console.WriteLine("Client is Receoived the XML data from the Master server");
            Console.WriteLine("=========================================================");
            Console.WriteLine("Type Table received from Master Server");
            Console.WriteLine(typeXML);
            Console.WriteLine("Package Dependency Table received from Master Server");
            Console.WriteLine(packageXML);
            //(new LinqDisplay()).displaythroughLinq(typeXML, packageXML);            
           string xml = constructOutput(typeXML, packageXML);
           Thread newWindowThread = new Thread(new ThreadStart(() =>
           {
               // Create and show the Window
               MainWindow window = new MainWindow(xml);
               //window.Content = xml;
               window.Show();
               // Start the Dispatcher Processing
               System.Windows.Threading.Dispatcher.Run();
           }));
           // Set the apartment state
           newWindowThread.SetApartmentState(ApartmentState.STA);
           // Make the thread a background thread
           //newWindowThread.IsBackground = true;
           // Start the thread
           newWindowThread.Start();
           //newWindowThread.Join();
        }

        public string constructOutput(string relationXml, string packageXml)
        {
            StringBuilder sb = new StringBuilder();
            if (null != relationXml && relationXml.Trim().Length>0)
            {
                sb.Append("Type Analysis Results").Append("\n");
                sb.Append("=====================").Append("\n");
                XDocument doc1 = XDocument.Parse(relationXml);
                var filename2 = from e in doc1.Elements("Relationships") select e;
                IEnumerable<XElement> elListr = from el in filename2.Descendants("File") select el;
                if (elListr == null || elListr.Count() == 0)
                {
                    sb.Append("No Type Analysis Results to Display").Append("\n\n\n");
                }
                foreach (XElement el in elListr)
                {
                    sb.Append("File name " + (string)el.Attribute("Name")).Append("\n");
                    IEnumerable<XElement> elList2 = from e in el.Elements("Class") select e;
                    foreach (XElement ell in elList2)
                    {
                        sb.Append("\tClass Name: " + (string)ell.Attribute("Name"));
                        sb.Append("\tDepends on Class:" + (string)ell.Attribute("RelatedTo"));
                        sb.Append("\tType of Relationship: " + (string)ell.Attribute("RelType"));
                        sb.Append("\n");
                    }
                }
                sb.Append("\n\n");
            }

            if (null != packageXml && packageXml.Trim().Length>0)
            {
                sb.Append("Package Analysis Results").Append("\n");
                sb.Append("========================").Append("\n");
                XDocument doc = XDocument.Parse(packageXml);
                var filename = from e in doc.Elements("PackageDependency") select e;
                IEnumerable<XElement> elList = from el in filename.Descendants("Package") select el;
                if(elList == null || elList.Count() == 0){
                    sb.Append("No Package Analysis Results to display").Append("\n\n\n");
                }
                foreach (XElement el in elList)
                {
                    sb.Append("Package Name" + (string)el.Attribute("Name")).Append("\n");
                    IEnumerable<XElement> elList2 = from e in el.Elements("DependOn") select e;
                    if (elList2.Count() == 0)
                    {
                        sb.Append("\tNo package dependencies\n");

                    }
                    foreach (XElement ell in elList2)
                        sb.Append("\tDepends On: " + (string)ell.Attribute("Name")).Append("\n");
                }
                sb.Append("\n\n");
            }
            return sb.ToString();
        }

    }
}
