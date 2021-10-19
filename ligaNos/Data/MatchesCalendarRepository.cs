using ligaNos.Data.Entities;
using ligaNos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public class MatchesCalendarRepository : GenericRepository<MatchCalendar>, IMatchesCalendarRepository
    {
        private readonly DataContext _context;
        private readonly IMatchesResultsTempRepository _tempRepository;

        public MatchesCalendarRepository(DataContext context, IMatchesResultsTempRepository tempRepository) : base(context)
        {
            _context = context;
            _tempRepository = tempRepository;
        }



        public async Task AddMatchesAsync(MatchesViewModel model)
        {
            if (_context.MatchesCalendar.Count() == 0 || _context.MatchesCalendar == null )
            {
               
                List<string> Sourting = new List<string>();
                List<int> Numbers = new List<int>();
                List<Club> SourtingClubs = new List<Club>();
                Random nr = new Random();

                var allClubs = _context.Clubs;
                foreach (var item in allClubs)
                {
                    Numbers.Add(item.Id);
                }
                int max = 18;
                while (Numbers.Count() >0 )
                {
                    int n = nr.Next(0, max);
                    max--;
                    if (Numbers[n] != null || Numbers[n] != 0)
                    {
                        var club = allClubs.Where(x => x.Id == Numbers[n]).FirstOrDefault();
                        Sourting.Add(club.Name);
                        Numbers.RemoveAt(n);
                    }
                }
        
                foreach (string item in Sourting)
                {
                    var find = _context.Clubs.Where(x => x.Name == item).FirstOrDefault();
                    SourtingClubs.Add(find);
                }

                List<Club> clubs = new List<Club>();
                clubs.AddRange(SourtingClubs);
                clubs.RemoveAt(0);

                int nrclubs = clubs.Count();
                int game = 0;
                int numberJourneyGame = 0;

                List<MatchCalendar> lista = new List<MatchCalendar>();

                DateTime date = DateTime.Today.AddDays(10);

                for (int i = 0; i < 34; i++)
                {
                    game++;
                    if (numberJourneyGame != 9)
                        numberJourneyGame++;
                    else
                        numberJourneyGame = 1;


                    int teamId = i % nrclubs;
                    if (i < 17)
                    {
                        lista.Add(new MatchCalendar
                        {
                            Journey = i + 1,
                            NumberGame = numberJourneyGame,
                            HomeTeamId = clubs[teamId].Id,
                            AwayTeamId = SourtingClubs[0].Id,
                            Date = date,      
                            Played = false
                        });

                    }
                    else
                    {
                        lista.Add(new MatchCalendar
                        {
                            Journey = i + 1,
                            NumberGame = numberJourneyGame,
                            HomeTeamId = SourtingClubs[0].Id,
                            AwayTeamId = clubs[teamId].Id,
                            Date = date,      
                            Played = false
                        });
                    }


                    for (int j = 1; j < 9; j++)
                    {
                        date = date.AddDays(1);
                        game++;
                        numberJourneyGame++;
                        int clubeCasa = (i + j) % nrclubs;
                        int clubeFora = (i + nrclubs - j) % nrclubs;

                        if (i < 17)
                        {
                            lista.Add(new MatchCalendar
                            {
                                Journey = i + 1,
                                NumberGame = numberJourneyGame,
                                HomeTeamId = clubs[clubeCasa].Id,
                                AwayTeamId = clubs[clubeFora].Id,
                                Date = date,      //mega batota, dat datas random
                                Played = false
                            });
                        }
                        else
                        {
                            lista.Add(new MatchCalendar
                            {
                                Journey = i + 1,
                                NumberGame = numberJourneyGame,
                                HomeTeamId = clubs[clubeFora].Id,
                                AwayTeamId = clubs[clubeCasa].Id,
                                Date = date,      //mega batota, dat datas random
                                Played = false
                            });
                        }
                    }
                  
                }

                foreach (var item in lista)
                {
                    await this.CreateAsync(item);

                   
                }
            }




        }


        //--------------------------------------------RESULTS
        public async Task<ResultTemp> GetResultsAsync(int id) // vai buscar os resultados temporarios, pq os temporarios sao sempre alterados e confirmados
        {
            return await _context.ResultTemps.FindAsync(id);
        }
    }
}
