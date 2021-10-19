using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Address { get; set; }


        [Required]
        [MaxLength(20, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } //nif

        //falta o cargooooooooooooo

        [Required]
        [Display(Name = "Tax Number")]
        [StringLength(9, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string TaxNumber { get; set; } //nif


    }
}
