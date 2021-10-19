using ligaNos.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ligaNos.Models
{
    public class RegisterNewUserViewModel : User
    {

        public int CountClubsSupreme;

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }


        [Required]
        [MinLength(6)] //o tamanho tem de ser igual em todo o lado para nao dar erro
        public string Password { get; set; }


        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

    }
}
