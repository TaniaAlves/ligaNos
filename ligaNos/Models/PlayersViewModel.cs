using ligaNos.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Models
{
    public class PlayersViewModel : Player
    {
        public int ClubId { get; set; }


        public int PlayerId { get; set; }

        public string PositionName { get; set; }
        public IEnumerable<SelectListItem> Positions { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
