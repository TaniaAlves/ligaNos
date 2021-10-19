using ligaNos.Data.Entities;
using ligaNos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public interface IMatchesCalendarRepository : IGenericRepository<MatchCalendar>
    {
        //IQueryable GetMatchesWithResults();
        Task AddMatchesAsync(MatchesViewModel model);

        Task<ResultTemp> GetResultsAsync(int id);
    }
}
