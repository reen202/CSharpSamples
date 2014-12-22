/////////////////////////////////////////////////////////////////////////
// ProgHost.cs - Service Host for Programmatic BasicService demo       //
//                                                                     //
//   Uses BasicHttpBinding                                             //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
//
// - Started with C# Console Application Project
// - Made reference to .Net System.ServiceModel
// - Added using System.ServiceModel
// - Made reference to IService dll
// - Made reference to Service dll
// - Added code to create communication channel

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.Threading;
using CodeAnalysis;


namespace HandCraftedService
{
    class Server1Host
    {     
        static void Main(string[] args)
        {
            List<string> serverUrlList = new List<string>();
            serverUrlList.Add(DependencyConstants.localhostUrlPrefix + DependencyConstants.slaveServer1PortNo +
            "/" + DependencyConstants.slaveServerPath);
            serverUrlList.Add(DependencyConstants.localhostUrlPrefix + DependencyConstants.salveServer2PortNo +
            "/" + DependencyConstants.slaveServerPath);
            Receiver receiver1 = new Receiver();
            receiver1.actMultipleRecievers(serverUrlList);            
        }
    }
}
