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
using CodeAnalysis;


namespace HandCraftedService
{
    class MasterServerHosting
    {      
        static void Main(string[] args)
        {
            Receiver receiver1 = new Receiver(DependencyConstants.localhostUrlPrefix+DependencyConstants.masterServerPortNo+
            "/"+DependencyConstants.masterServerPath);
            receiver1.actReceiver(DependencyConstants.masterServer1Name);
            Console.ReadKey();
        }
    }
}
