using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{

    public class ServerToMaster
    {
        private RequestDetails requestDetails_;

        public RequestDetails requestDetails
        {
            get { return requestDetails_; }
            set { requestDetails_ = value; }
        }

        private TypeTable typeTable_;

        public TypeTable typeTable
        {
            get { return typeTable_; }
            set { typeTable_ = value; }
        }

        private Dictionary<String, List<ElemRelation>> relTable_;

        public Dictionary<String, List<ElemRelation>> relTable
        {
            get { return relTable_; }
            set { relTable_ = value; }
        }

        private Dictionary<string, List<string>> packageAnalysis_;

        public Dictionary<string, List<string>> packageAnalysis
        {
            get { return packageAnalysis_; }
            set { packageAnalysis_ = value; }
        }
        public ServerToMaster()
        {
            packageAnalysis = new Dictionary<string, List<string>>();
        }
        static void Main(string[] args)
        {
        }
    }

    public class RequestDetails
    {

        private string clientName_;

        public string clientName
        {
            get { return clientName_; }
            set { clientName_ = value; }
        }

        private string clientUrl_;

        public string clientUrl
        {
            get { return clientUrl_; }
            set { clientUrl_ = value; }
        }

        private string serverName_;

        public string serverName
        {
            get { return serverName_; }
            set { serverName_ = value; }
        }

        private List<string> serverList_;

        public List<string> serverList
        {
            get { return serverList_; }
            set { serverList_ = value; }
        }

        private List<string> receivedSrvList_;

        public List<string> receivedSrvList
        {
            get { return receivedSrvList_; }
            set { receivedSrvList_ = value; }
        }

        private string[] args_;

        public string[] args
        {
            get { return args_; }
            set { args_ = value; }
        }

    }
}
