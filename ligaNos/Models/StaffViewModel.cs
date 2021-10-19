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
    public class StaffViewModel : Staff
    {
        public int ClubId { get; set; }
        public int StaffId { get; set; }
        //public int OccupationId { get; set; }

        public string OccupationName { get; set; }
        public IEnumerable<SelectListItem> Occupations { get; set; }
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
