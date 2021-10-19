using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data.Entities
{
    public class ClubResult : IEntity
    {
        public int Id { get; set; }
        public Club Club { get; set; }
        //public int ClubId { get; set; }
        public int MarkedGoals { get; set; }
        public int SufferedGoals { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int Victorys { get; set; }
        public int Losts { get; set; }
        public int Ties { get; set; }
        public int Pontuation { get; set; }
    }
}
