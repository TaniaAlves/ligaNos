using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public class OccupationRepository : GenericRepository<Occupation>, IOccupationRepository
    {
        private readonly DataContext _context;
        public OccupationRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboOccupation()
        {
            var list = _context.Occupations.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "(Select a position...)",
                Value = "0"
            });

            return list;
        }
    }
}
