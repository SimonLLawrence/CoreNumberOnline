using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Model
{
    public interface IApiSecrets
    {
        public string SecretId { get; set; }
        public string Secret { get; set; }
        public string Key { get; set; }
        public string SubaccountName { get; set; }
    }
}
