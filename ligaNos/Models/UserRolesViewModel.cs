﻿using ligaNos.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Models
{
    public class UserRolesViewModel : User
    {
        public int CountClubsSupreme;

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
