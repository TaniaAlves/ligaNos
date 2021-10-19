using ligaNos.Data;
using ligaNos.Data.Entities;
using ligaNos.Helpers;
using ligaNos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ligaNos.Controllers
{
    public class ClubManagerController : Controller
    {
        private readonly DataContext _context;
        private readonly IClubResultsRepository _clubResultsRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;  //video 33
        private readonly IConfiguration _configuration; //video 33 para aceder ao json do token
        private readonly UserManager<User> _userManager;

        public ClubManagerController(
            DataContext context,
            IClubResultsRepository clubResultsRepository,
            IClubRepository clubRepository,
            IUserHelper userHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper,
            IConfiguration configuration,
            UserManager<User> userManager)
        {
            _context = context;
            _clubResultsRepository = clubResultsRepository;
            _clubRepository = clubRepository;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _userManager = userManager;
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ClubManagerList() 
        {
            var users = await _userManager.GetUsersInRoleAsync("TeamManager");

            var userRolesViewModel = new List<RegisterNewUserViewModel>();
           
            foreach (User user in users)
            {
                var model = new RegisterNewUserViewModel();
                model.Id = user.Id;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PostalCode = user.PostalCode;
                model.TaxNumber = user.TaxNumber;
                model.PhoneNumber = user.PhoneNumber;
                model.Email = user.Email;
                model.ImageUrl = user.ImageUrl;
                var clubs = await _userHelper.GetClubManagerWithClubAsync(user.Id);
                model.CountClubsSupreme = clubs.NumberClubs;
               
               userRolesViewModel.Add(model);
            }

            return View(userRolesViewModel);
        }

        //-------------------------------------ADD CLUB
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddClub(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var clubManager = await _userHelper.GetUserByIdAsync(id);
            if (clubManager == null)
            {
                return NotFound();
            }

            var clubs = await _userHelper.GetClubManagerWithClubAsync(id);
            if (clubs.NumberClubs == 1)
            {
                return RedirectToAction("ExistingClub", "Errors");
            }


            var model = new CreateNewClubViewModel { ClubManagerId = clubManager.Id };
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddClub (CreateNewClubViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                var name = _clubRepository.GetClubByName(model.Name);
                if(name != null )
                    return RedirectToAction("ClubName", "Errors");

                var path = string.Empty;
                if(model.ImageFile != null && model.ImageFile.Length >0 )
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile, "clubs");
                }

                await _userHelper.AddClubAsync(model, path);
                var lastclubadded = _clubResultsRepository.GetLastClub();
                
                var clubR = new ClubResult
                {
                    Club = lastclubadded,
                    MarkedGoals = 0,
                    SufferedGoals = 0,
                    YellowCards = 0,
                    RedCards = 0,
                    Victorys = 0,
                    Losts = 0,
                    Ties = 0,
                    Pontuation =0,
                };
                await _clubResultsRepository.CreateAsync(clubR);

                return RedirectToAction("DetailsClubManager", new { id = model.ClubManagerId }); 
            }
            

            return this.View(model);
        }


        //-------------------------------------DELETE CLUB
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClub(int id)     
        {
            var findP =await _clubRepository.GetClubWithPlayersAsync(id);
            var findS = await _clubRepository.GetClubWithStaffAsync(id);
            var sourtedMatch = _context.MatchesCalendar.ToList();

            if (findP.Players.Count() == 0 && findS.Staffs.Count() ==0 && sourtedMatch.Count() ==0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var club = await _userHelper.GetClubAsync(id);
                if (club == null)
                {
                    return NotFound();
                }

                await _clubResultsRepository.DeleteClubResultsAsync(club);
                var clubId = await _userHelper.DeleteClubAsync(club);
                
                return RedirectToAction($"DetailsClubManager", new { id = clubId });
            }
            return RedirectToAction("InfoAggregatedClub", "Errors");

        }

        //-------------------------------------EDIT CLUB
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditClub(int? id)
        {

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
                Name = club.Name,
                Stadium = club.Stadium,
                Address = club.Address,
                PostalCode = club.PostalCode,
                Email = club.Email,
                ImageUrl = club.ImageUrl,
            };
            return View(clubView);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditClub(CreateNewClubViewModel model)
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
                    return this.RedirectToAction($"DetailsClubManager", new { id = managerId });
                }
            }

            return this.View(model);
        }




        //----------------------------------------DELETE CLUB MANAGER
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClubManager(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var findC = await _userHelper.GetClubManagerWithClubAsync(id);
            if(findC.Clubs.Count() == 0 )
            {
                var emp = await _userManager.FindByIdAsync(id);
                if (emp == null)
                {
                    return NotFound();
                }

                await _userManager.DeleteAsync(emp);
                return RedirectToAction(nameof(ClubManagerList));
            }

            return RedirectToAction("ClubInManager", "Errors");

        }



        //// ----------------------------------------EDIT CLUB MANAGER
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditClubManager(string? id)   //metodo VIEW para alterar dados do user, rato direito para criar a view
        {
            var user = await _userHelper.GetUserByIdAsync(id);
            var model = new RegisterNewUserViewModel();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address; 
                model.PhoneNumber = user.PhoneNumber;
                model.TaxNumber = user.TaxNumber;
                model.PostalCode = user.PostalCode;
                model.ImageUrl = user.ImageUrl;
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditClubManager(RegisterNewUserViewModel model, string id) //metodo POST para alterar os dados efetivos
        {
            if (!ModelState.IsValid) 
            {
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile, "users");
                }
                var user = await _userHelper.GetUserByIdAsync(id);
                if (user != null) //se ele existir vai alterar os dados
                {

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;  //video 32  
                    user.PhoneNumber = model.PhoneNumber;
                    user.PostalCode = model.PostalCode;
                    user.TaxNumber = model.TaxNumber;
                    user.ImageUrl = path;

                    var response = await _userHelper.UpdateUserAsync(user);
                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }

            return View(model);
        }


        //// ----------------------------------------DETAILS CLUB MANAGER
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsClubManager(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubManager = await _userHelper.GetClubManagerWithClubAsync(id);
            if (clubManager == null)
            {
                return NotFound();
            }

            var cmView = new RegisterNewUserViewModel
            {
                Id = clubManager.Id,
                FirstName = clubManager.FirstName,
                LastName = clubManager.LastName,
                Address = clubManager.Address,
                PostalCode = clubManager.PostalCode,
                TaxNumber = clubManager.TaxNumber,
                PhoneNumber = clubManager.PhoneNumber,
                Clubs = clubManager.Clubs,
                ImageUrl = clubManager.ImageUrl,

            };

            return View(cmView);
        }



        //// ----------------------------------------ADD CLUB MANAGER
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterClubManager()
        {
            return View(); 
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RegisterClubManager(RegisterNewUserViewModel model) 
        {
            var users = await _userManager.GetUsersInRoleAsync("TeamManager");
            if (ModelState.IsValid && users.Count != 18)
            {
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile, "users");
                }
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)       
                {

                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        TaxNumber = model.TaxNumber,
                        PostalCode = model.PostalCode,
                        ImageUrl = path
                    };


                    var result = await _userHelper.AddUserAsync(user, model.Password); 
                    if (result != IdentityResult.Success)      
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }
 
                    await _userHelper.AddUserToRoleAsync(user, "TeamManager"); 

                    //com o envio de email e com token:                                                
                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "User", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    Response response = _mailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");


                    if (response.IsSuccess)
                    {
                        ViewBag.Message = "The instructions to allow you user has been sent to email";
                        return View(model);
                    }

                    ModelState.AddModelError(string.Empty, "The user couldn't be logged.");

                }
            }

            return View(model);
        }
     

        ////-------------------------------------------- TOKEN

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)  //video 33 para gerar o token, por norma é sempre igual
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded) // se pass estiver correta
                    {
                        var claims = new[] //claims- tipo de objeto que ele usa para fazer esta autorização
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //gera credenciais
                        var token = new JwtSecurityToken(   //objeto token
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15), //podemos por o tempo que quisermos
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);

                    }
                }
            }

            return BadRequest();
        }
    }
}
