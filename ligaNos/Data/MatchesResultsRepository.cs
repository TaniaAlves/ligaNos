using ligaNos.Data.Entities;
using ligaNos.Helpers;
using ligaNos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public class MatchesResultsRepository : GenericRepository<Result>, IMatchesResultsRepository
    {
        private readonly DataContext _context;
        private readonly ClubRepository _clubRepository;
        private readonly IUserHelper _userHelper;

        public MatchesResultsRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<bool> ConfirmResultsAsync(int id) 
        {
            //var user = await _userHelper.GetUserByEmailAsync(userName);
            //if (user == null)
            //{
            //    return false;
            //}

            var resultTemp =  _context.ResultTemps 
                .Where(o => o.IdMatch == id).FirstOrDefault();

            if (resultTemp == null) 
            {
                return false;
            }

            //var order = orderTmps.Select(o => new Result 
            //{
            //    MGHome = o.MGHome,
            //    MGAway = o.MGAway,
            //    YCHome = o.YCHome,
            //    YCAway = o.YCAway,
            //    RCHome = o.RCHome,
            //    RCAway = o.RCAway,
            //    PontuationHome = o.PontuationHome,
            //    PontuationAway = o.PontuationAway,
            //    IdMatch = o.IdMatch,
            //    Id = o.Id,
            //}); 

            var res = new Result //fazemos o order associado
            {

                MGHome = resultTemp.MGHome,
                MGAway = resultTemp.MGAway,
                YCHome = resultTemp.YCHome,
                YCAway = resultTemp.YCAway,
                RCHome = resultTemp.RCHome,
                RCAway = resultTemp.RCAway,
                PontuationHome = resultTemp.PontuationHome,
                PontuationAway = resultTemp.PontuationAway,
                IdMatch = resultTemp.IdMatch,
                //Id = resultTemp.Id,
                

                //OrderDate = DateTime.UtcNow, //para ir buscar a hora de qdo fizeram a encomenda
                //User = user,
                //Items = details //em que leva a lista de items do OrderDetails
            };

            await CreateAsync(res); //criar a order


          
            

            await _context.SaveChangesAsync(); //salvar as alterações
            return true;
        }

    }
}
