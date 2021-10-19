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
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly IImageHelper _imageHelper;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;  //video 33
        private readonly IConfiguration _configuration; //video 33 para aceder ao json do token
        private readonly UserManager<User> _userManager;

        public EmployeesController(IUserHelper userHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper,
          IConfiguration configuration,
          UserManager<User> userManager)
        {
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()  //lista de empregados
        {
            var users = await _userManager.GetUsersInRoleAsync("GamesManager");
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
                userRolesViewModel.Add(model);
            }

            return View(userRolesViewModel);
        }


        //----------------------------------------DELETE EMPLOYEE
        public async Task<IActionResult> DeleteEmployee(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emp = await _userManager.FindByIdAsync(id);
            if (emp == null)
            {
                return NotFound();
            }
            
            await _userManager.DeleteAsync(emp);
            return RedirectToAction(nameof(Index));
        }

        //// ----------------------------------------EDIT EMPLOYEE

        public async Task<IActionResult> EditEmployee(string? id)   //metodo VIEW para alterar dados do user, rato direito para criar a view
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


        [HttpPost]
        public async Task<IActionResult> EditEmployee(RegisterNewUserViewModel model, string id) //metodo POST para alterar os dados efetivos
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


        //// ----------------------------------------DETAILS EMPLOYEE
        public async Task<IActionResult> DetailsEmployee(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emp = await _userHelper.GetClubManagerWithClubAsync(id);
            if (emp == null)
            {
                return NotFound();
            }

            var empview = new RegisterNewUserViewModel
            {
                Id = emp.Id,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Address = emp.Address,
                PostalCode = emp.PostalCode,
                TaxNumber = emp.TaxNumber,
                PhoneNumber = emp.PhoneNumber,

                ImageUrl = emp.ImageUrl,
                
            };


            return View(empview);
        }


        // ----------------------------------------ADD EMPLOYEE

        public IActionResult RegisterEmployees()
        {
           
            return View(); //se nao retorna para a view
        }



        [HttpPost]
        public async Task<IActionResult> RegisterEmployees(RegisterNewUserViewModel model) //Post do registo para o efetuar
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile, "users");
                }
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)       // se nao existir vamos criar 
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

                    var result = await _userHelper.AddUserAsync(user, model.Password); //adicionar user
                    if (result != IdentityResult.Success)       //se nao conseguiu criar manda msg de erro
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }


                    //se utilizador estiver logado -> roles da drop down list selecionada
                    //caso contrário: 
                    await _userHelper.AddUserToRoleAsync(user, "GamesManager"); //video 19 a criar um role de cliente a este user



                    //com o envio de email e com token:                                                 //video 33
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


        //-------------------------------------------- TOKEN

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
