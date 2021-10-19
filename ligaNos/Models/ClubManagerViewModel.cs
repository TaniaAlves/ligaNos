using ligaNos.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Models
{
    public class ClubManagerViewModel 
    {
        public int ClubId { get; set; }


        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }


        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Address { get; set; }


        [Required]
        [MaxLength(20, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(8, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string PostalCode { get; set; } 


        [Required]
        [Display(Name = "Tax Number")]
        [StringLength(9, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string TaxNumber { get; set; } 

        [Required]
        [MinLength(6)] //o tamanho tem de ser igual em todo o lado para nao dar erro
        public string Password { get; set; }


        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }

    }
}
