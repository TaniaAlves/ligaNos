﻿using ligaNos.Data.Entities;
using ligaNos.Helpers;
using ligaNos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ligaNos.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;  //video 33
        private readonly IConfiguration _configuration; //video 33 para aceder ao json do token

        public UserController(IUserHelper userHelper,
            IMailHelper mailHelper,
            IImageHelper imageHelper,
          IConfiguration configuration  )
        {
            _imageHelper = imageHelper;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
        }



        //---------------------------------------------- LOGIN -------------------------------------------------
        // GET: UserController
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated && this.User.IsInRole("Admin")) 
                return RedirectToAction("Index", "Dashboard");
            else if(User.Identity.IsAuthenticated )
                return RedirectToAction("Index", "Home");

            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) 
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                        return Redirect(this.Request.Query["ReturnUrl"].First());

                    if (User.Identity.IsAuthenticated && this.User.IsInRole("Admin")) 
                        return this.RedirectToAction("Index", "Dashboard");
                    //else if (User.Identity.IsAuthenticated)
                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login!"); 
            return View(model);
        }

        public async Task<IActionResult> Logout() 
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home"); 
        }

        // ----------------------------------------ADD USER

        public IActionResult Register()
        {
            return View(); 
        }


        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model) 
        {
            if (ModelState.IsValid)
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
                        ImageUrl = path,
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password); 
                    if (result != IdentityResult.Success)      
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }

                    await _userHelper.AddUserToRoleAsync(user, "Client"); 
                           
                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "User", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    Response response = _mailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>"+
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

        //------------------------------- EMAIL CONFIRMATION
        public async Task<IActionResult> ConfirmEmail(string userId, string token)  
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
               
            }

            return View();
        }

        public async Task<IActionResult> ConfirmEmailEmployees(string userId, string token)  
        {

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
               
            }

            return RedirectToAction("ResetPassword", "User");

        }
        //-------------------------- CHANGE USER AND PASSWORD
        public async Task<IActionResult> ChangeUser()  
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
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
        public async Task<IActionResult> ChangeUser(RegisterNewUserViewModel model) 
        {
            if (!ModelState.IsValid) 
            {
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile, "users");
                }


                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null) 
                {

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;  
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


        public IActionResult ChangePassword()
        {
            return View(); 
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) 
        {
            if (ModelState.IsValid) 
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return this.View(model);
        }


     

      
        //-----------------------------RECOVER PASSWORD W/TOKEN
        public IActionResult RecoverPassword() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model) 
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action(
                    "ResetPassword",
                    "User",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(model.Email, "ligaNos Password Reset", $"<h1>ligaNos Password Reset</h1>" +
                $"To reset the password click in this link:</br></br>" +
                $"<a href = \"{link}\">Reset Password</a>");

                if (response.IsSuccess)
                {
                    this.ViewBag.Message = "The instructions to recover your password has been sent to email.";
                }

                return this.View();

            }

            return this.View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) 
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    this.ViewBag.Message = "Password reset successful.";
                    return View();
                }

                this.ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            this.ViewBag.Message = "User not found.";
            return View(model);
        }





        //-------------------------------------------- TOKEN

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model) 
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded) 
                    {
                        var claims = new[] 
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(   
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15), 
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
