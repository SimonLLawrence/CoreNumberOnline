using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Repository
{
    public interface IBotInstanceDataRepository
    {
        BotInstanceData GetBotInstanceData(string id);
        IEnumerable<BotInstanceData> GetAllBotInstanceData();
        void Save(BotInstanceData botInstanceData);
    }
}
