using ligaNos.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Models
{
    public class ClubResultsViewModel : ClubResult
    {
        //public Club Club { get; set; }
        public User User { get; set; }
    }
}
