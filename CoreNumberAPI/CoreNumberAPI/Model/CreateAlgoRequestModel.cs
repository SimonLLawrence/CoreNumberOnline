﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Model
{
    public class CreateAlgoRequestModel
    {
        public string AlgoName { get; set; }
        public string ExchangeName { get; set; }
        public string Key { get; set; } 
        public string Secret { get; set; }
        public string Subaccount { get; set; }
    }
}
