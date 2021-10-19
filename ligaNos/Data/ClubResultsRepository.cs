using ligaNos.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public class ClubResultsRepository : GenericRepository<ClubResult>, IClubResultsRepository
    {
        private readonly DataContext _context;
        public ClubResultsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> DeleteClubResultsAsync(Club club)
        {
            var clubr = await _context.ClubResults
                .Where(c => c.Club.Id == club.Id)
                .FirstOrDefaultAsync();
            if (club == null)
            {
                return 0;
            }

            _context.ClubResults.Remove(clubr);
            await _context.SaveChangesAsync();
            return club.Id;
        }



        public Club GetLastClub()
        {
            return _context.Clubs
            .OrderByDescending(c => c.Id).FirstOrDefault();
        }

        public IEnumerable<ClubResult> GetClubResults()
        {
            return _context.ClubResults
                .Include(x => x.Club)
            .OrderBy(c => c.Id);
        }

        public async Task<int> UpdateClubResultsAsync(int clubId, int tempId, MatchCalendar match)
        {
            var clubsr = this.GetClubResults();
            var clubr = clubsr.Where(c => c.Club.Id == clubId).FirstOrDefault();
            if (clubr == null)
            {
                return 0;
            }

            var results = await _context.ResultTemps.Where(x => x.IdMatch == tempId).FirstOrDefaultAsync();

            if (clubr.Club.Id == match.HomeTeamId)
            {
                clubr.MarkedGoals += results.MGHome;
                clubr.SufferedGoals += results.MGAway;
                clubr.YellowCards += results.YCHome;
                clubr.RedCards += results.RCHome;
                clubr.Pontuation += results.PontuationHome;

                if (results.PontuationHome == 3)
                {
                    clubr.Victorys += 1;
                    clubr.Ties += 0;
                    clubr.Losts += 0;
                }
                else if (results.PontuationHome == 1)
                {
                    clubr.Victorys += 0;
                    clubr.Ties += 1;
                    clubr.Losts += 0;
                }
                else
                {
                    clubr.Victorys += 0;
                    clubr.Ties += 0;
                    clubr.Losts += 1;
                }
            }
            else
            {
                clubr.MarkedGoals += results.MGAway;
                clubr.SufferedGoals += results.MGHome;
                clubr.YellowCards += results.YCAway;
                clubr.RedCards += results.RCAway;
                clubr.Pontuation += results.PontuationAway;

                if (results.PontuationAway == 3)
                {
                    clubr.Victorys += 1;
                    clubr.Ties += 0;
                    clubr.Losts += 0;
                }
                else if (results.PontuationHome == 1)
                {
                    clubr.Victorys += 0;
                    clubr.Ties += 1;
                    clubr.Losts += 0;
                }
                else
                {
                    clubr.Victorys += 0;
                    clubr.Ties += 0;
                    clubr.Losts += 1;
                }
            }

            _context.ClubResults.Update(clubr);
            await _context.SaveChangesAsync();
            return clubr.Id;
        }

        public async Task<IEnumerable<ClubResult>> ResetClubResultsAsync()
        {
            var clubsr = this.GetClubResults();
            //var clubr = clubsr.Where(c => c.Club.Id == clubId).FirstOrDefault();
            if (clubsr == null)
            {
                return null;
            }

            foreach (var item in clubsr)
            {
                item.MarkedGoals = 0;
                item.SufferedGoals = 0;
                item.YellowCards = 0;
                item.RedCards = 0;
                item.Pontuation = 0;
                item.Victorys = 0;
                item.Ties = 0;
                item.Losts = 0;
                _context.ClubResults.Update(item);
               
            }

            await _context.SaveChangesAsync();
            return clubsr;
        }
    }
}
