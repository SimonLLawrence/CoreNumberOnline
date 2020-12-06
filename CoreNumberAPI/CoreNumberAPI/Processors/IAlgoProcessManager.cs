using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Processors
{
    interface IAlgoProcessManager
    {
        string State { get; set; }
        void StartProcessingAlgos();
        void StopProcessingAlgos();
        void ExecuteAlgo();

    }
}
