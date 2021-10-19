using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data.Entities
{
    public class Player : IEntity
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string FirstName { get; set; }


        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string LastName { get; set; }


        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";


        [MaxLength(2, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string Number { get; set; }


        public int PositionId { get; set; }

        public Position Position { get; set; }

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

        //interligação para resultados
    }
}
