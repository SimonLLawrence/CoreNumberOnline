using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;
using CoreNumberAPI.Repository;
using CoreNumberAPI.Services;

namespace CoreNumberAPI.Processors
{
    public interface IBotProcessor
    {
        string BotProcessorName { get; }
        void Initialise(BotInstanceData instance);
        void Process(BotInstanceData instance, DateTime timeOfProcess);
        void Shutdown(BotInstanceData instance);
    }
}
