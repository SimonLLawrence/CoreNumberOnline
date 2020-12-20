using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;
using ServiceStack.Redis;

namespace CoreNumberAPI.Repository
{
    public class RedisSecretDataRepository : ISecretDataRepository
    {
        private readonly IRedisClientsManager _redisManager;

        public RedisSecretDataRepository(IRedisClientsManager redisManager)
        {
            _redisManager = redisManager;
        }

        public IApiSecrets GetApiSecret(string secretId)
        {
            using var redis = _redisManager.GetClient();
            var redisBotInstanceData = redis.As<IApiSecrets>();
            return redisBotInstanceData.GetHash<string>("SECRET_DATA").FirstOrDefault(i => i.Key == secretId).Value;
        }

        public void Save(IApiSecrets secret)
        {
            using var redis = _redisManager.GetClient();
            var redisBotInstanceData = redis.As<IApiSecrets>();
            var hash = redisBotInstanceData.GetHash<string>("SECRET_DATA");
            hash.Add(secret.SecretId, secret);
            redisBotInstanceData.Save();
        }
    }
}
