using ligaNos.Data;
using ligaNos.Data.Entities;
using ligaNos.Helpers;
using ligaNos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Controllers
{
    public class MatchesController : Controller
    {

        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IClubRepository _clubRepository;
        private readonly IClubResultsRepository _clubResultsRepository;
        private readonly IMatchesCalendarRepository _matchesRepository;
        private readonly IMatchesResultsRepository _matchesResultsRepository;
        private readonly IMatchesResultsTempRepository _matchesResultsTempRepository;


        public MatchesController(
            IClubRepository clubRepository,
            IClubResultsRepository clubResultsRepository,
            IUserHelper userHelper,
            IMatchesResultsTempRepository matchesResultsTempRepository,
            IMatchesCalendarRepository matchesRepository,
            IMatchesResultsRepository matchesResultsRepository,
            DataContext context)
        {
            _clubRepository = clubRepository;
            _clubResultsRepository = clubResultsRepository;
            _context = context;
            _matchesRepository = matchesRepository;
            _matchesResultsRepository = matchesResultsRepository;
            _matchesResultsTempRepository = matchesResultsTempRepository;
            _userHelper = userHelper;

        }

        [Authorize(Roles = "GamesManager")]
        public IActionResult Matches()
        {
            List<MatchCalendar> list = new List<MatchCalendar>();
            List<MatchesViewModel> list2 = new List<MatchesViewModel>();
            foreach (var item in _context.MatchesCalendar)
            {
                list.Add(item);
            }
            foreach (var item in list)
            {
                var findHome = _context.Clubs.Where(c => c.Id == item.HomeTeamId).FirstOrDefault();
                var findAway = _context.Clubs.Where(c => c.Id == item.AwayTeamId).FirstOrDefault();
                list2.Add(new MatchesViewModel
                {
                    Journey = item.Journey,
                    NumberGame = item.NumberGame,
                    HomeTeam = findHome, 
                    AwayTeam = findAway,
                    ImageUrlHome = findHome.ImageUrl,
                    ImageUrlAway = findAway.ImageUrl,
                    Date = item.Date,     
                    Played = false
                });
            }
            
            return View(list2);
        }

        [Authorize(Roles = " GamesManager")]
        //[HttpPost]
        public async Task<IActionResult> AddMatches(MatchesViewModel match)    
        {
            int clubs = _context.Clubs.Count();
            if (ModelState.IsValid && clubs == 18)
            {
                await _matchesRepository.AddMatchesAsync(match);

                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                var games = _context.MatchesCalendar;
                var tempresults = _context.ResultTemps.Count();
                if (tempresults == 0)
                {
                    List<ResultTemp> list = new List<ResultTemp>();                
                    foreach (var item in games)
                    {
                        list.Add(new ResultTemp
                        {
                            User = user,
                            UserId = user.Id,
                            IdMatch = item.Id,
                            MGHome = 0,
                            MGAway = 0,
                            YCHome = 0,
                            YCAway = 0,
                            RCHome = 0,
                            RCAway = 0,
                            PontuationHome = 0,
                            PontuationAway = 0,

                        });
                    }
                    foreach (var item in list)
                    {
                        await _matchesResultsTempRepository.CreateAsync(item);

                    }
                }
            }
            else
                this.ModelState.AddModelError(string.Empty, "Sorry but you need 18 clubs to create matches.");

            return RedirectToAction("Matches"); 

        }
        [Authorize(Roles = " GamesManager")]
        public async Task<IActionResult> DeleteMatches()    
        {
            List<MatchCalendar> list = new List<MatchCalendar>();

                foreach (var item in _context.MatchesCalendar)
                {
                    list.Add(item);
                }
                foreach (var item in list)              //fazer reset nos club results
                {
                    var find = _context.ResultTemps.Where(x => x.IdMatch == item.Id).FirstOrDefault();
                    await _matchesResultsTempRepository.DeleteAsync(find);
                    await _matchesRepository.DeleteAsync(item);
                }
            await _clubResultsRepository.ResetClubResultsAsync();

            return RedirectToAction("Matches");
        }



        //-------------------------------VIEW RESULTS
        [Authorize(Roles = "Admin, GamesManager")]
        public IActionResult GamesResults()            //se os ids da match nao correspondem entao editar os temps
        {
            List<MatchCalendar> game = new List<MatchCalendar>();
            List<ResultTemp> list = new List<ResultTemp>();
            List<GameResultsViewModel> list2 = new List<GameResultsViewModel>();
            var user = _userHelper.GetUserByEmailAsync(this.User.Identity.Name).Result;
            foreach (var item in _context.MatchesCalendar)
            {
                game.Add(item);
            }
            foreach (var item in _context.ResultTemps)
            {
                list.Add(item);
            }
            foreach (var item in game)
            {
                var findHome = _context.Clubs.Where(c => c.Id == item.HomeTeamId).FirstOrDefault();
                var findAway = _context.Clubs.Where(c => c.Id == item.AwayTeamId).FirstOrDefault();
                var findGameResult = list.Where(c => c.IdMatch == item.Id).FirstOrDefault();

                if(findGameResult != null )
                {
                    list2.Add(new GameResultsViewModel
                    {
                        User = user,
                        Id = item.Id,
                        IdMatch = item.Id,
                        HomeTeam = findHome,
                        AwayTeam = findAway,
                        ImageUrlHome = findHome.ImageUrl,
                        ImageUrlAway = findAway.ImageUrl,
                        MGHome = findGameResult.MGHome,
                        MGAway = findGameResult.MGAway,
                        YCHome = findGameResult.YCHome,
                        YCAway = findGameResult.YCAway,
                        RCHome = findGameResult.RCHome,
                        RCAway = findGameResult.RCAway,
                        PontuationHome = findGameResult.PontuationHome,
                        PontuationAway = findGameResult.PontuationAway,
                        Played = item.Played,

                    }) ;
                }
                else
                {
                    list2.Add(new GameResultsViewModel
                    {
                        User = user,
                        Id = item.Id,
                        IdMatch = item.Id,
                        HomeTeam = findHome,
                        AwayTeam = findAway,
                        ImageUrlHome = findHome.ImageUrl,
                        ImageUrlAway = findAway.ImageUrl,
                        MGHome = 0,
                        MGAway = 0,
                        YCHome = 0,
                        YCAway = 0,
                        RCHome = 0,
                        RCAway = 0,
                        PontuationHome = 0,
                        PontuationAway = 0,
                        Played = false,

                    }) ;
                }
            }

            return View(list2);  
        }



        //------------------------------- ADDING TEMPORARY RESULTS
        [Authorize(Roles = "Admin, GamesManager")]
        public async Task<IActionResult> DetailsGamesResult(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var game =  _context.MatchesCalendar.Where(c=> c.Id == id).FirstOrDefault();
          
            var idresult = _context.ResultTemps.Where(c => c.IdMatch == id).FirstOrDefault().IdMatch;

            
            var match = _context.MatchesCalendar.Where(c => c.Id == id).FirstOrDefault();  //vai buscar o match para saber se foi jogado
            bool played = match.Played;
            if (match == null)
            {
                played = false;
            }
          

            var findHome = _context.Clubs.Where(c => c.Id == game.HomeTeamId).FirstOrDefault();
            var findAway = _context.Clubs.Where(c => c.Id == game.AwayTeamId).FirstOrDefault();
            var findGameResult = _context.ResultTemps.Where(c => c.IdMatch == id).FirstOrDefault();   //okay
            var findUserGame = _userHelper.GetUserByIdAsync(findGameResult.UserId).Result;

            var list2 = new GameResultsViewModel  // com os resultados temp guardados
            {
                User = findGameResult.User,
                UserName = findUserGame.FirstName,
                Id = idresult,
                IdMatch = game.Id,
                HomeTeam = findHome,
                AwayTeam = findAway,
                ImageUrlHome = findHome.ImageUrl,
                ImageUrlAway = findAway.ImageUrl,
                MGHome = findGameResult.MGHome,
                MGAway = findGameResult.MGAway,
                YCHome = findGameResult.YCHome,
                YCAway = findGameResult.YCAway,
                RCHome = findGameResult.RCHome,
                RCAway = findGameResult.RCAway,
                PontuationHome = findGameResult.PontuationHome,
                PontuationAway = findGameResult.PontuationAway,
                Played = played
            };
            return View(list2);
        }

        [Authorize(Roles = "Admin, GamesManager")]
        public async Task<IActionResult> EditResultHome(int? id)        
        {                                
            if (id == null)
            {
                return NotFound();
            }

            var idgame = _context.ResultTemps.Where(c => c.IdMatch == id).FirstOrDefault();
            
            var gameResult = await _matchesRepository.GetResultsAsync(idgame.Id); //temp
            if (gameResult == null)
            {
                return NotFound();
            }
            
            var game = _context.MatchesCalendar.Where(c => c.Id == id).FirstOrDefault();
            var findHome = _context.Clubs.Where(c => c.Id == game.HomeTeamId).FirstOrDefault();
            var findAway = _context.Clubs.Where(c => c.Id == game.AwayTeamId).FirstOrDefault();
            var findUserGame = _userHelper.GetUserByIdAsync(idgame.UserId).Result;

            var gameViewModel = new GameResultsViewModel
            {
                User = idgame.User,
                UserName = findUserGame.FirstName,
                Id = gameResult.Id,
                IdMatch = gameResult.IdMatch,
                HomeTeam = findHome,
                AwayTeam = findAway,
                ImageUrlHome = findHome.ImageUrl,
                ImageUrlAway = findAway.ImageUrl,
                MGHome = gameResult.MGHome,
                MGAway = gameResult.MGAway,
                YCHome = gameResult.YCHome,
                YCAway = gameResult.YCAway,
                RCHome = gameResult.RCHome,
                RCAway = gameResult.RCAway,
                PontuationHome = gameResult.PontuationHome,
                PontuationAway = gameResult.PontuationAway,
                Played = false,
            };
            return View(gameViewModel);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, GamesManager")]
        public async Task<IActionResult> EditResultHome(GameResultsViewModel game) 
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                var idgame = _context.ResultTemps.Where(c => c.IdMatch == game.Id).FirstOrDefault();

                var list = new ResultTemp
                {
                    User = user,
                    IdMatch = idgame.IdMatch,
                    MGHome = game.MGHome,
                    MGAway = game.MGAway,
                    YCHome = game.YCHome,
                    YCAway = game.YCAway,
                    RCHome = game.RCHome,
                    RCAway = game.RCAway,
                    PontuationHome = game.PontuationHome,
                    PontuationAway = game.PontuationAway,
                };
                var clubId = await _matchesResultsTempRepository.UpdateResultsHomeAsync(list);

                // search for teams 
                var match = _context.MatchesCalendar.Where(c => c.Id == game.Id).FirstOrDefault();
                //to update club results individually
                await _clubResultsRepository.UpdateClubResultsAsync(match.HomeTeamId, clubId, match);
                await _clubResultsRepository.UpdateClubResultsAsync(match.AwayTeamId, clubId, match);

                if (clubId != 0)                                           
                {
                    return this.RedirectToAction($"DetailsGamesResult", new { id = clubId });
                }
            }

            return this.View(game);
        }


        //-------------------------------- CONFIRM RESULTS
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmResults(int id)     
        {
            var response = await _matchesResultsRepository.ConfirmResultsAsync(id);
            if (response)
            {
               
                var game = _context.MatchesCalendar
                  .Where(o => o.Id == id).FirstOrDefault();
                game.Played = true;            
                await _matchesRepository.UpdateAsync(game);

               
                return RedirectToAction("GamesResults");
            }

            return RedirectToAction("GamesResults");
        }


        //-------------------------------------RATINGS
        [Authorize(Roles = "Admin, TeamManager, GamesManager")]
        public IActionResult Ratings()                    
        {
            List<Result> results = new List<Result>();
           
            List<ClubResultsViewModel> list = new List<ClubResultsViewModel>();

            var FinalResults = _context.Results;
            var games = _context.MatchesCalendar;
           
            var clubs = _context.Clubs;
           

            foreach (var club in clubs)
            {
                int mg = 0, sg = 0, yc = 0, rc = 0, v = 0, d = 0, e = 0, pts = 0;
                foreach (var item in games)
                {
                    var gr = _context.ResultTemps.Where(x => x.IdMatch == item.Id).FirstOrDefault();
                    if(gr != null)
                    {
                        if (club.Id == item.HomeTeamId)
                        {
                            mg += gr.MGHome;
                            sg += gr.MGAway;
                            yc += gr.YCHome;
                            rc += gr.RCHome;
                            pts += gr.PontuationHome;
                            if(gr.PontuationHome == 3)
                            v +=  1;
                            else if(gr.PontuationHome == 0)
                            d += 1;
                            else
                            e += 1;
                        }
                        if (club.Id == item.AwayTeamId)
                        {
                            mg += gr.MGAway;
                            sg += gr.MGHome;
                            yc += gr.YCAway;
                            rc += gr.RCAway;
                            pts += gr.PontuationAway;
                            if (gr.PontuationAway == 3)
                                v += 1;
                            else if (gr.PontuationAway == 0)
                                d += 1;
                            else
                                e += 1;
                        }
                    }
                    else
                    {
                        mg += 0;
                        sg += 0;
                        yc += 0;
                        rc += 0;
                        pts += 0;
                        v += 0;
                        d += 0;
                        e += 0;
                    }
                   
                }

                list.Add(new ClubResultsViewModel
                {
                    Club = club,
                    //Id = .Id,
                    //ClubId = club.Id,
                    MarkedGoals = mg,
                    SufferedGoals = sg,
                    YellowCards = yc,
                    RedCards = rc,
                    Victorys = v,
                    Losts = d,
                    Ties = e,
                    Pontuation = pts,

                }) ;
            }
            list.OrderByDescending(x => x.Pontuation);
            return View(list);
        }


        [Authorize(Roles = "Admin, TeamManager, GamesManager")]
        public IActionResult Statistics()
        {
            var results =  _clubResultsRepository.GetClubResults();
            var user = _userHelper.GetUserByEmailAsync(this.User.Identity.Name).Result;
            List<ClubResultsViewModel> list = new List<ClubResultsViewModel>();

            foreach (var item in results)
            {
                var club = _context.Clubs.Where(x => x.Id == item.Club.Id).FirstOrDefault();
                list.Add(new ClubResultsViewModel
                {
                    User = user,
                    Club = club,
                    //ClubId = club.Id,
                    MarkedGoals = item.MarkedGoals,
                    SufferedGoals = item.SufferedGoals,
                    YellowCards = item.YellowCards,
                    RedCards = item.RedCards,
                    Victorys = item.Victorys,
                    Losts = item.Losts,
                    Ties = item.Ties,
                    Pontuation = item.Pontuation,
                }) ;
            }
            list.OrderByDescending(x => x.Pontuation);
            return View(list);
        }

    }
}
