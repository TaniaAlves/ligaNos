using ligaNos.Data.Entities;
using ligaNos.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ligaNos.Helpers
{
    public interface IUserHelper
    {
        //Task<IEnumerable<SelectListItem>> GetUsers(string role);

        //----------------------ADD USERS

        Task<IdentityResult> AddUserAsync(User user, string password); //para quando o cliente se regista

        //----------------------LOGIN /LOGOUT

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        //----------------------ROLES
        Task CheckRoleAsync(string roleName);
        Task AddUserToRoleAsync(User user, string roleName); //para adicionar um role ao user

        Task<bool> IsUserInRoleAsync(User user, string roleName); //ve se o user tem role

        //----------------------TOKEN -> EMAIL TO CONFIRM REGISTER

        Task<SignInResult> ValidatePasswordAsync(User user, string password);  //video 33

        Task<string> GenerateEmailConfirmationTokenAsync(User user); //video 33

        Task<IdentityResult> ConfirmEmailAsync(User user, string token); //video 33
        Task<User> GetUserByIdAsync(string userId); //para termos o user pelo id no email

        Task<User> GetUserByEmailAsync(string email);

        //----------------------RECOVER PASSWORD W/TOKEN
        Task<string> GeneratePasswordResetTokenAsync(User user); //video 34

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password); //video 34

        //---------------------CHANGE USER AND PASSWORD
        Task<IdentityResult> UpdateUserAsync(User user); //video 18 , ligaçao para view Change User
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword); //ligação para view Cange Password
       
        //--------------------- GET CLUB MANAGER WITH CLUB
        Task<User> GetClubManagerWithClubAsync(string id);

        

        //-------------------ADD CLUB
        Task AddClubAsync(CreateNewClubViewModel model, string path);


        // -------------------DELETE CLUB
        Task<Club> GetClubAsync(int id);
        Task<string> DeleteClubAsync(Club club);

        //------------------- UPDATE CLUB
        Task<string> UpdateClubAsync(Club club, string path);
    }
}
