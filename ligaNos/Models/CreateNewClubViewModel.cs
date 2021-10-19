using ligaNos.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Models
{
    public class CreateNewClubViewModel : Club
    {
        public string ClubManagerId { get; set; }
        public int ClubId { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        //Icollection staff (com Representante Clube)
        //Icollection jogadores
    }
}
