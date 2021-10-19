using ligaNos.Data.Entities;
using ligaNos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public interface IMatchesResultsRepository : IGenericRepository<Result>
    {
        //Task<Result> GetResultsAsync(int id);
        Task<bool> ConfirmResultsAsync(int id); 
    }
}
