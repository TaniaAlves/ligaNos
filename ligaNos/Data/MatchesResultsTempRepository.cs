using ligaNos.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public class MatchesResultsTempRepository : GenericRepository<ResultTemp>, IMatchesResultsTempRepository
    {
        private readonly DataContext _context;
        private readonly ClubRepository _clubRepository;

        public MatchesResultsTempRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> UpdateResultsHomeAsync(ResultTemp result) //TEMPORARIO
        {
           

            var resultTemp = _context.ResultTemps.Where(c => c.IdMatch == result.IdMatch).FirstOrDefault();
            if (result.MGHome > result.MGAway)
            {
                resultTemp.PontuationHome = 3;
                resultTemp.PontuationAway = 0;
            }
            else if (result.MGHome == result.MGAway)
            {
                resultTemp.PontuationHome = 1;
                resultTemp.PontuationAway = 1;
            }
            else
            {
                resultTemp.PontuationHome = 0;
                resultTemp.PontuationAway = 3;
            }
            resultTemp.User = result.User;
            resultTemp.UserId = result.User.Id;
            resultTemp.MGAway = result.MGAway;
            resultTemp.MGHome = result.MGHome;
            resultTemp.YCAway = result.YCAway;
            resultTemp.YCHome = result.YCHome;
            resultTemp.RCAway = result.RCAway;
            resultTemp.RCHome = result.RCHome;

            _context.ResultTemps.Update(resultTemp);
            await _context.SaveChangesAsync();
            return resultTemp.IdMatch;
        }

    }
}
