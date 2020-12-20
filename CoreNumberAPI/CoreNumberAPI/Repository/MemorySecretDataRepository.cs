using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Repository
{
    public class MemorySecretDataRepository : ISecretDataRepository
    {
        private static readonly Dictionary<string,IApiSecrets> Data = new Dictionary<string, IApiSecrets>();

        public MemorySecretDataRepository(ApiSecrets testSecret)
        {
            if (testSecret != null)
            {
                Data.Add(testSecret.SecretId, testSecret);
            }
        }

        public IApiSecrets GetApiSecret(string secretId)
        {
            return Data.GetValueOrDefault(secretId);
        }

        public void Save(IApiSecrets secret)
        {
            Data[secret.SecretId] = secret;
        }
    }
}
 