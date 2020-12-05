using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Repository
{
    interface ISymbolDataRepository
    {
        SymbolData GetSymbolData(Guid id);
        IEnumerable<SymbolData> GetAllSymbolData();
        void Save(SymbolData symboldate);
    }
}
