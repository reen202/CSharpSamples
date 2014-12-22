using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class DependencyConstants
    {
        //private static readonly string  server1Name = "Server 1";
        private static readonly string masterServerUrl_ = "http://localhost:8082/MasterServerService";

        public static string masterServerUrl
        {
            get { return DependencyConstants.masterServerUrl_; }
        }

        private static readonly string masterServer1Name_ = "Master Server 1";

        public static string masterServer1Name
        {
            get { return DependencyConstants.masterServer1Name_; }
        }

        private static readonly string slaveServer1Name_ = "Slave Server 1";

        public static string slaveServer1Name
        {
            get { return DependencyConstants.slaveServer1Name_; }
        }

        private static readonly string slaveServer2Name_ = "Slave Server 2";

        public static string slaveServer2Name
        {
            get { return DependencyConstants.slaveServer2Name_; }
        }

        private static readonly string masterServerPath_ = "MasterServerService";

        public static string masterServerPath
        {
            get { return DependencyConstants.masterServerPath_; }
        }

        private static readonly string slaveServerPath_ = "ServerService";

        public static string slaveServerPath
        {
            get { return DependencyConstants.slaveServerPath_; }
        }

        private static readonly string clientPath_ = "ClientService";

        public static string clientPath
        {
            get { return DependencyConstants.clientPath_; }
        }

        private static readonly string localhostUrlPrefix_ = "http://localhost:";

        public static string localhostUrlPrefix
        {
            get { return DependencyConstants.localhostUrlPrefix_; }
        }

        private static readonly string masterServerPortNo_ = "9000";

        public static string masterServerPortNo
        {
            get { return DependencyConstants.masterServerPortNo_; }
        }

        private static readonly string slaveServer1PortNo_ = "8090";

        public static string slaveServer1PortNo
        {
            get { return DependencyConstants.slaveServer1PortNo_; }
        }

        private static readonly string salveServer2PortNo_ = "8091";

        public static string salveServer2PortNo
        {
            get { return DependencyConstants.salveServer2PortNo_; }
        } 


    }
}
