using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Repository
{
    public class MemoryBotInstanceDataRepository : IBotInstanceDataRepository
    {
        private static Dictionary<string,BotInstanceData> Data = new Dictionary<string, BotInstanceData>();
        
        public BotInstanceData GetBotInstanceData(string id)
        {
            return Data.GetValueOrDefault(id);
        }

        public IEnumerable<BotInstanceData> GetAllBotInstanceData()
        {
            return Data.Values;
        }

        public void Save(BotInstanceData botInstanceData)
        {
            Data[botInstanceData.Id.ToString()] = botInstanceData;
        }
    }
}
