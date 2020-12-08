using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Repository
{
    public interface IAlgoInstanceDataRepository
    {
        AlgoInstanceData GetAlgoInstanceData(string id);
        IEnumerable<AlgoInstanceData> GetAllAlgoInstanceData();
        void Save(AlgoInstanceData algoInstanceData);
    }
}
