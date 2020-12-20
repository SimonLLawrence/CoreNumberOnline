using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;
using ServiceStack.Redis;

namespace CoreNumberAPI.Repository
{
    public class RedisBotInstanceDataRepository : IBotInstanceDataRepository
    {
        private readonly IRedisClientsManager _redisManager;

        public RedisBotInstanceDataRepository(IRedisClientsManager redisManager)
        {
            _redisManager = redisManager;
        }

        public BotInstanceData GetBotInstanceData(string id)
        {
            using var redis = _redisManager.GetClient();
            var redisBotInstanceData = redis.As<BotInstanceData>();
            return redisBotInstanceData.GetHash<string>("BOT_INSTANCE_DATA").FirstOrDefault(i => i.Key == id).Value;
        }

        public IEnumerable<BotInstanceData> GetAllBotInstanceData()
        {
            using var redis = _redisManager.GetClient();
            var redisBotInstanceData = redis.As<BotInstanceData>();
            var hash = redisBotInstanceData.GetHash<string>("BOT_INSTANCE_DATA");
            return redisBotInstanceData.GetAllEntriesFromHash(hash).Values;
        }

        public void Save(BotInstanceData botInstanceData)
        {
            using var redis = _redisManager.GetClient();
            var redisBotInstanceData = redis.As<BotInstanceData>();
            var hash = redisBotInstanceData.GetHash<string>("BOT_INSTANCE_DATA");
            hash.Add(botInstanceData.Id.ToString(), botInstanceData);
            redisBotInstanceData.Save();
            
        }
    }
}
