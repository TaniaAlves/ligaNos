using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data.Entities
{
    public class MatchCalendar : IEntity
    {
        public int Id { get; set; }

        public int Journey { get; set; }

        public int NumberGame { get; set; }

        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }


        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }

        public bool Played { get; set; }


    }
}
