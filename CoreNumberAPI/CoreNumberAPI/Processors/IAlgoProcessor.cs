using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;
using CoreNumberAPI.Repository;
using CoreNumberAPI.Services;

namespace CoreNumberAPI.Processors
{
    public interface IAlgoProcessor
    {
        string AlgorithmName { get; }
        void Initialise(AlgoInstanceData instance);
        void Process(AlgoInstanceData instance, DateTime timeOfProcess);
        void Shutdown(AlgoInstanceData instance);
    }
}
