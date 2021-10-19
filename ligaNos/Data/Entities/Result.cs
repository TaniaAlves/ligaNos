using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data.Entities
{
    public class Result : IEntity
    {
        public int Id { get; set; }
        public int IdMatch { get; set; }
        public int MGHome { get; set; }
        public int MGAway { get; set; }
        public int YCHome { get; set; }
        public int YCAway { get; set; }
        public int RCHome { get; set; }
        public int RCAway { get; set; }
        public int PontuationHome { get; set; }
        public int PontuationAway { get; set; }
    }
}
