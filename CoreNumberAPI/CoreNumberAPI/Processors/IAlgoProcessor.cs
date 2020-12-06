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
        void Process(AlgoInstanceData instance, IExchange exchange, IApiSecrets secret, IAlgoInstanceDataRepository algoInstanceRepository , DateTime utcNow);
    }
}
