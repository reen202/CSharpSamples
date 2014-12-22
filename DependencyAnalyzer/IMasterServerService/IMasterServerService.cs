﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using CodeAnalysis;

namespace HandCraftedService
{
    [ServiceContract(Namespace = "HandCraftedService")]
    public interface IMasterServerService
    {
        [OperationContract]
         void resolveDependencyInTypeTables(ServerToMaster serverToMasters);
    }

}
