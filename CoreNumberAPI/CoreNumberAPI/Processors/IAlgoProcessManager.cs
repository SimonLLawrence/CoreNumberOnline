using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Processors
{
    interface IAlgoProcessManager
    {
        string State { get; set; }
        void StartProcessingAlgos();
        void StopProcessingAlgos();
        void ExecuteAllAlgos();
        void ExecuteAlgo(AlgoInstanceData instance);
        void StartAlgo(string algoInstanceId);
        void StopAlgo(string algoInstanceId);
        string CreateAlgo(string algoName, string exchangeName, string secretId);
        void DestroyAlgo(string algoInstanceId);
    }
}
