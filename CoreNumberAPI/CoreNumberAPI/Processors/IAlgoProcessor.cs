using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Processors
{
    interface IAlgoProcessor
    {
        string AlgorithmName { get; }
        public void Process(DateTime executionTime);
    }
}
