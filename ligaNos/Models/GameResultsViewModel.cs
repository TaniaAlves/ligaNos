using ligaNos.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Models
{
    public class GameResultsViewModel : Result
    {
        //public int MatchesId { get; set; }

        public User User { get; set; }
        public bool Played { get; set; }  // para controlar os que posso editar
        public string UserName { get; set; }

        public Club HomeTeam { get; set; }
        public Club AwayTeam { get; set; }
        [Display(Name = "Image")]
        public IFormFile ImageFileHome { get; set; }
        [Display(Name = "Image")]
        public IFormFile ImageFileAway { get; set; }

        public string ImageUrlHome { get; set; }
        public string ImageFullPathHome
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrlHome))
                {
                    return null;
                }

                return $"https://localhost:44340{ImageUrlHome.Substring(1)}";
            }
        }

        public string ImageUrlAway { get; set; }
        public string ImageFullPathAway
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrlAway))
                {
                    return null;
                }

                return $"https://localhost:44340{ImageUrlAway.Substring(1)}";
            }
        }
    }
}
