using ligaNos.Data;
using ligaNos.Data.Entities;
using ligaNos.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager; //video 19, para tratar dos roles de cada user


        public UserHelper(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }


        //------------------------- ADD USER
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }



        //----------------------------LOGIN / LOGOUT
        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                false); //se meter true ele se nos enganarmos na pass ou assim ja nao nos deixa entrar
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        //--------------------- ROLES ------------------------


        public async Task CheckRoleAsync(string roleName)   //implementaçao task para verificar se o user tem um role
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName); //se existir role vai busca-lo
            if (!roleExists)                                //se nao tiver um role vai criar
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)  //19- para ver se o user tem role
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task AddUserToRoleAsync(User user, string roleName) // para adicionar o role ao user, video 19
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }


        //---------------------------TOKEN -> email to confirm new register
        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(
               user,
               password,
               false); // para n bloquear apos x tentativas
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }


        // ------------------RECOVER PASSWORD W/TOKEN
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        //----------------------------- CHANGE USER AND PASSWORD
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);    // metodo para mudar dados de utilizador
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword); //alteração de password
        }


        //--------------GET CLUB MANAGER WHITH CLUB
        public async Task<User> GetClubManagerWithClubAsync(string id)
        {
            return await _userManager.Users
                .Include(c => c.Clubs)
                .Where(c => c.Id == id )
                .FirstOrDefaultAsync();
        }


        //------------------------ADD CLUB
        public async Task AddClubAsync(CreateNewClubViewModel model, string path)
        {
            var clubManager = await this.GetClubManagerWithClubAsync(model.ClubManagerId.ToString());
  
            if (clubManager == null)
                return;

            clubManager.Clubs.Add(new Club
            {
                Name = model.Name,
                Stadium = model.Stadium,
                Address = model.Address,
                PostalCode = model.PostalCode,
                Email = model.Email,
                ImageUrl = path,
            });

            await _userManager.UpdateAsync(clubManager);
            await _context.SaveChangesAsync();
            
        }


        //--------------------DELETE CLUB
        public async Task<Club> GetClubAsync(int id)
        {
            return await _context.Clubs.FindAsync(id);
        }

        public async Task<string> DeleteClubAsync(Club club)
        {
            var clubManager = await _userManager.Users
              .Where(c => c.Clubs.Any(ci => ci.Id == club.Id))
              .FirstOrDefaultAsync();
            if (clubManager == null)
            {
                return "";
            }

            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();
            return clubManager.Id;
        }


        //--------------------UPDATE CLUB
        public async Task<string> UpdateClubAsync(Club club, string path)
        {
            var manager = await _userManager.Users
            .Where(c => c.Clubs.Any(ci => ci.Id == club.Id)).FirstOrDefaultAsync();
            if (manager == null)
            {
                return "";
            }

            club.ImageUrl = path;
            _context.Clubs.Update(club);
            await _context.SaveChangesAsync();
            return manager.Id;
        }

        //------------------------- 
        //public async Task<IEnumerable<SelectListItem>> GetUsers(string role)
        //{
        //    return (IEnumerable<SelectListItem>)await _userManager.GetUsersInRoleAsync(role);
        //}
    }
}
