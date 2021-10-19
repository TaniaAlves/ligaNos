using ligaNos.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public interface IPositionRepository : IGenericRepository<Position>
    {
        IEnumerable<SelectListItem> GetComboPositions();   //video 32
    }
}
