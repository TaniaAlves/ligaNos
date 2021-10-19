using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data.Entities
{
    public class PlayerResult : IEntity
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int Goals { get; set; }

        public int YellowCards { get; set; }
        public int RedCards { get; set; }
    }
}
