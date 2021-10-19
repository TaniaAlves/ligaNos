using ligaNos.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public class PositionRepository : GenericRepository<Position>, IPositionRepository
    {
        private readonly DataContext _context;
        public PositionRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboPositions()
        {
            var list = _context.Positions.Select(c => new SelectListItem
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
