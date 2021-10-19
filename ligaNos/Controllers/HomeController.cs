using ligaNos.Data;
using ligaNos.Data.Entities;
using ligaNos.Helpers;
using ligaNos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClubRepository _clubRepository;
        private readonly IClubResultsRepository _clubResultsRepository;
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public HomeController(ILogger<HomeController> logger, IClubRepository clubRepository, 
            DataContext context, IClubResultsRepository clubResultsRepository,
            IUserHelper userHelper)
        {
            _logger = logger;
            _clubRepository = clubRepository;
            _clubResultsRepository = clubResultsRepository;
            _context = context;
            _userHelper = userHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ------------------- view teams

        public IActionResult Teams()   
        {
            return View(_clubRepository.GetClubWithPlayers());
        }

        //-------------------- view matches

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


        //------------------------- view Results
        public IActionResult GamesResults()                     // se o jogo ja foi jogado ent a opção de jogar n aparece
        {
            List<MatchCalendar> game = new List<MatchCalendar>();
            List<ResultTemp> list = new List<ResultTemp>();
            List<GameResultsViewModel> list2 = new List<GameResultsViewModel>();
            //var user = _userHelper.GetUserByEmailAsync(this.User.Identity.Name).Result;
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

                if (findGameResult != null)
                {
                    list2.Add(new GameResultsViewModel
                    {
                        User = null,
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

                    });
                }
                else
                {
                    list2.Add(new GameResultsViewModel
                    {
                        User = null,
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

                    });
                }

            }

            return View(list2);
        }

        //-------------------------------------RATINGS
        public IActionResult Ratings()
        {
            List<Result> results = new List<Result>();
            //List<ResultTemp> temps = new List<ResultTemp>();
            List<ClubResultsViewModel> list = new List<ClubResultsViewModel>();

            var FinalResults = _context.Results;
            var games = _context.MatchesCalendar;

            var clubs = _context.Clubs;


            foreach (var club in clubs)
            {
                int mg = 0, sg = 0, yc = 0, rc = 0, v = 0, d = 0, e = 0, pts = 0;
                foreach (var item in games)
                {
                    var gr = _context.Results.Where(x => x.IdMatch == item.Id).FirstOrDefault();
                    if (gr != null)
                    {
                        if (club.Id == item.HomeTeamId)
                        {
                            mg += gr.MGHome;
                            sg += gr.MGAway;
                            yc += gr.YCHome;
                            rc += gr.RCHome;
                            pts += gr.PontuationHome;
                            if (gr.PontuationHome == 3)
                                v += 1;
                            else if (gr.PontuationHome == 0)
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
                    //Club = Club.Id,
                    MarkedGoals = mg,
                    SufferedGoals = sg,
                    YellowCards = yc,
                    RedCards = rc,
                    Victorys = v,
                    Losts = d,
                    Ties = e,
                    Pontuation = pts,

                });
            }
            list.OrderByDescending(x => x.Pontuation);
            return View(list);
        }

        public async Task<IActionResult> ClubDetails(int? id)
        {
            //var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            //string idUser = user.Id;
            //var clubs = await _userHelper.GetClubManagerWithClubAsync(idUser);

            //foreach (var item in clubs.Clubs)
            //{
            //    id = item.Id;
            //}

            if (id == null)
            {
                return NotFound();
            }

            var club = await _clubRepository.GetClubWithPlayersAsync(id.Value);
            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        //----------------------------- statistics

        public IActionResult Statistics()
        {
            var results = _clubResultsRepository.GetClubResults();
            //var user = _userHelper.GetUserByEmailAsync(this.User.Identity.Name).Result;
            List<ClubResultsViewModel> list = new List<ClubResultsViewModel>();

            foreach (var item in results)
            {
                var club = _context.Clubs.Where(x => x.Id == item.Club.Id).FirstOrDefault();
                list.Add(new ClubResultsViewModel
                {
                    //User = user,
                    Club = club,
                    //Club = club.Id,
                    MarkedGoals = item.MarkedGoals,
                    SufferedGoals = item.SufferedGoals,
                    YellowCards = item.YellowCards,
                    RedCards = item.RedCards,
                    Victorys = item.Victorys,
                    Losts = item.Losts,
                    Ties = item.Ties,
                    Pontuation = item.Pontuation,
                });
            }
            list.OrderByDescending(x => x.Pontuation);
            return View(list);
        }
    }
}
