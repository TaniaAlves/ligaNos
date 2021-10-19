using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ligaNos.Data.Entities
{
    public class User : IdentityUser
    {
        //public int ClubId { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string FirstName { get; set; }


        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string LastName { get; set; }


        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string Address { get; set; }        

        //atençao falta por a imagem


        [StringLength(9, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string TaxNumber { get; set; } //nif

        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<Club> Clubs { get; set; }

        [Display(Name = "Number of Clubs")]
        public int NumberClubs => Clubs == null ? 0 : Clubs.Count;

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

        //public Club Club { get; set; }

        //public int ClubsId { get; set; }
    }
}
