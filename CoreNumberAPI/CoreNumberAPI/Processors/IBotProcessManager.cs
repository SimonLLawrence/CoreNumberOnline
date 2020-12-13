using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Processors
{
    public interface IBotProcessManager
    {
        string State { get; set; }
        void StartProcessingBots();
        void StopProcessingBots();
        void ExecuteAllAlgos();
        void ExecuteBot(string botInstanceId);
        void StartBot(string botInstanceId);
        void StopBot(string botInstanceId);
        string CreateBot(string botProcessorName, string exchangeName, string key, string secret, string subaccount);
        string CreateSecret(string key, string secret, string subaccount);
        Dictionary<string,string> GetConfiguration(string botInstanceId);
        void SetConfiguration(string botInstanceId, Dictionary<string,string> variables);
        void AddTradingViewUpdate(string botInstanceId, Dictionary<string, string> payload);
        void ShutdownBot(string botInstanceId);
    }
}
