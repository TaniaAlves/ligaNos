using ligaNos.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public interface IMatchesResultsTempRepository :IGenericRepository<ResultTemp>
    {
        Task<int> UpdateResultsHomeAsync(ResultTemp result);
    }
}
