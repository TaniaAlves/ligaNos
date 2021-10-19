using ligaNos.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public interface IClubResultsRepository : IGenericRepository<ClubResult>
    {
        Club GetLastClub();
        IEnumerable<ClubResult> GetClubResults();
        Task<int> DeleteClubResultsAsync(Club club);

        Task<int> UpdateClubResultsAsync(int clubId, int tempId, MatchCalendar match);
        Task<IEnumerable<ClubResult>> ResetClubResultsAsync();
    }
}
