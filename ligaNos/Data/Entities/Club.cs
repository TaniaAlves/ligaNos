using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data.Entities
{
    public class Club : IEntity
    {

        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string Name { get; set; }


        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string Stadium { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string Address { get; set; }

        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public ICollection<Player> Players { get; set; }

        [Display(Name = "Number of Players")]
        public int NumberPlayers => Players == null ? 0 : Players.Count;

        public ICollection<Staff> Staffs { get; set; }

        [Display(Name = "Number of Staff")]
        public int NumberStaff => Staffs == null ? 0 : Staffs.Count;


        public string ImageUrl { get; set; }
        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }

                return $"https://localhost:44340{ImageUrl.Substring(1)}";
            }
        }


        //public User ClubManager { get; set; }

        //public int ClubManagerId { get; set; }

        //public string President { get {return _userManager.FindByNameAsync(string id) }; 
        //    set; }


        //public User User { get; set; }

        //public int ClubManagerId { get; set; }
        //Icollection jogadores
        //ICollection<User> ClubManagers { get; set; }

    }
}
