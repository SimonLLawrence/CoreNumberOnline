using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Processors
{
    public interface IAlgoProcessManager
    {
        string State { get; set; }
        void StartProcessingAlgos();
        void StopProcessingAlgos();
        void ExecuteAllAlgos();
        void ExecuteAlgo(string algoInstanceId);
        void StartAlgo(string algoInstanceId);
        void StopAlgo(string algoInstanceId);
        string CreateAlgo(string algoName, string exchangeName, string key, string secret, string subaccount);
        string CreateSecret(string key, string secret, string subaccount);
        void ShutdownAlgo(string algoInstanceId);
    }
}
