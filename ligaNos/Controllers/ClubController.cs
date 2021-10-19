using ligaNos.Data;
using ligaNos.Data.Entities;
using ligaNos.Helpers;
using ligaNos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ligaNos.Controllers
{
    public class ClubController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly DataContext _context;
        private readonly IMailHelper _mailHelper;  //video 33
        private readonly IConfiguration _configuration; //video 33 para aceder ao json do token
        private readonly UserManager<User> _userManager;
        private readonly IClubRepository _clubRepository; //1º
        private readonly IPositionRepository _positionRepository; //1º
        private readonly IOccupationRepository _occupationRepository;
        //private readonly IFlashMessage _flashMessage; //video 36

        public ClubController(
            IClubRepository clubRepository,
            IPositionRepository positionRepository,
            IUserHelper userHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper,
            IConfiguration configuration,
            IOccupationRepository occupationRepository,
            DataContext context,
             UserManager<User> userManager)   //IFlashMessage flashMessage)
        {
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _userManager = userManager;
            _clubRepository = clubRepository;
            _positionRepository = positionRepository;
            _occupationRepository = occupationRepository;
            _context = context;
            //_flashMessage = flashMessage; //video 36
        }

        [Authorize(Roles = "Admin, TeamManager, GamesManager")]
        public IActionResult Index()   //video 31 1º
        {
            return View(_clubRepository.GetClubWithPlayers());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }


        [Authorize(Roles = "Admin, TeamManager")]
        public async Task<IActionResult> Edit(int? id)
        {
           
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            string idUser = user.Id;
            var clubs = await _userHelper.GetClubManagerWithClubAsync(idUser);

            foreach (var item in clubs.Clubs)
            {
                id = item.Id;
            }

            if (id == null)
            {
                return NotFound();
            }

            var club = await _userHelper.GetClubAsync(id.Value);
            if (club == null)
            {
                return NotFound();
            }

            var clubView = new CreateNewClubViewModel
            {
                Id = club.Id,
                ClubId = club.Id,
                Name = club.Name,
                Stadium = club.Stadium,
                Address = club.Address,
                PostalCode = club.PostalCode,
                Email = club.Email,
                ImageUrl = club.ImageUrl,
            };
            return View(clubView);
        }


        [Authorize(Roles = "Admin, TeamManager")]
        [HttpPost]
        public async Task<IActionResult> Edit(CreateNewClubViewModel model)
        {

            if (this.ModelState.IsValid)
            {
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile, "clubs");
                }

                var managerId = await _userHelper.UpdateClubAsync(model, path);
                if (managerId != "")
                {
                    return this.RedirectToAction($"Edit", new { id = managerId });
                }
            }

            return this.View(model);

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _clubRepository.GetByIdAsync(id.Value);
            if (club == null)
            {
                return NotFound();
            }

            await _clubRepository.DeleteAsync(club);
            return RedirectToAction(nameof(Index));
        }




        //---------------------------------PLAYERS CONTROL
        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            string idUser = user.Id;
            var clubs = await _userHelper.GetClubManagerWithClubAsync(idUser);

            foreach (var item in clubs.Clubs)
            {
                id = item.Id;
            }

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

        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> AddPlayers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _clubRepository.GetByIdAsync(id.Value);
            if (club == null)
            {
                return NotFound();
            }

            var model = new PlayersViewModel { ClubId = club.Id, Positions = _positionRepository.GetComboPositions()};
            return View(model);
        }

        [Authorize(Roles = "TeamManager")]
        [HttpPost]
        public async Task<IActionResult> AddPlayers(PlayersViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile, "players");
                }

                await _clubRepository.AddPlayersAsync(model, path);
                return RedirectToAction("Details", new { id = model.ClubId });
            }

            return this.View(model);
        }

        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> EditPlayer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _clubRepository.GetPlayersAsync(id.Value);
            if (player == null)
            {
                return NotFound();
            }
            var playerViewModel = new PlayersViewModel
            {
                FirstName = player.FirstName,
                LastName = player.LastName,
                Number = player.Number,
                PositionId = player.PositionId,
                Positions = _positionRepository.GetComboPositions(),
                ImageUrl = player.ImageUrl,
            };
            return View(playerViewModel);
        }

        [Authorize(Roles = "TeamManager")]
        [HttpPost]
        public async Task<IActionResult> EditPlayer(PlayersViewModel player)
        {
            if (this.ModelState.IsValid)
            {
                var path = string.Empty;
                if (player.ImageFile != null && player.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UpLoadImageAsync(player.ImageFile, "players");
                }
                var clubId = await _clubRepository.UpdatePlayersAsync(player, path);
                if (clubId != 0)
                {
                    return this.RedirectToAction($"Details", new { id = clubId });
                }
            }

            return this.View(player);
        }

        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> DetailsPlayer(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _clubRepository.GetPlayersAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            var positionName = _clubRepository.GetPositionName(player.PositionId);

            var playerViewModel = new PlayersViewModel
            {
                FirstName = player.FirstName,
                LastName = player.LastName,
                Number = player.Number,
                PositionId = player.PositionId,
                PositionName = positionName.Result, 
                ImageUrl = player.ImageUrl,
            };
            return View(playerViewModel);
        }

        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> DeletePlayer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _clubRepository.GetPlayersAsync(id.Value);
            if (player == null)
            {
                return NotFound();
            }

            var clubId = await _clubRepository.DeletePlayersAsync(player);
            return this.RedirectToAction($"Details", new { id = clubId });
        }


        //---------------------------------STAFF CONTROL
        //----------------------- tentar mostrar o staff
        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> DetailsWStaff(int? id)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            string idUser = user.Id;
            var clubs = await _userHelper.GetClubManagerWithClubAsync(idUser);

            foreach (var item in clubs.Clubs)
            {
                id = item.Id;
            }

            if (id == null)
            {
                return NotFound();
            }

            var club = await _clubRepository.GetClubWithStaffAsync(id.Value);
            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> AddStaff(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _clubRepository.GetByIdAsync(id.Value);
            if (club == null)
            {
                return NotFound();
            }

            var model = new StaffViewModel { ClubId = club.Id, Occupations = _occupationRepository.GetComboOccupation() };
            return View(model);
        }

        [Authorize(Roles = "TeamManager")]
        [HttpPost]
        public async Task<IActionResult> AddStaff(StaffViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile, "staffs");
                }
                await _clubRepository.AddStaffAsync(model, path);
                return RedirectToAction("DetailsWStaff", new { id = model.ClubId });
            }

            return this.View(model);
        }

        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> EditStaff(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _clubRepository.GetStaffAsync(id.Value);
            if (staff == null)
            {
                return NotFound();
            }
            var staffViewModel = new StaffViewModel
            {
                StaffId = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Address = staff.Address,
                PostalCode = staff.PostalCode,
                TaxNumber = staff.TaxNumber,
                OccupationId = staff.OccupationId,
                ImageUrl = staff.ImageUrl,
                Occupations = _occupationRepository.GetComboOccupation()
            };
            return View(staffViewModel);
        }


        [Authorize(Roles = "TeamManager")]
        [HttpPost]
        public async Task<IActionResult> EditStaff(StaffViewModel staff)
        {
            if (this.ModelState.IsValid)
            {
                var path = string.Empty;
                if (staff.ImageFile != null && staff.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UpLoadImageAsync(staff.ImageFile, "staffs");
                }
                var clubId = await _clubRepository.UpdateStaffAsync(staff, path);
                if (clubId != 0)
                {
                    return this.RedirectToAction($"DetailsWStaff", new { id = clubId });
                }
            }

            return this.View(staff);
        }

        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> DetailsStaff(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _clubRepository.GetStaffAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            var occupationName = _clubRepository.GetOccupationName(staff.OccupationId);

            var staffViewModel = new StaffViewModel
            {
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Address = staff.Address,
                PostalCode = staff.PostalCode,
                TaxNumber = staff.TaxNumber,
                OccupationId = staff.OccupationId,
                OccupationName = occupationName.Result,
                ImageUrl = staff.ImageUrl,
            };
            return View(staffViewModel);
        }

        [Authorize(Roles = "TeamManager")]
        public async Task<IActionResult> DeleteStaff(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _clubRepository.GetStaffAsync(id.Value);
            if (staff == null)
            {
                return NotFound();
            }

            var clubId = await _clubRepository.DeleteStaffAsync(staff);
            return this.RedirectToAction($"DetailsWStaff", new { id = clubId });
        }

    }
}
