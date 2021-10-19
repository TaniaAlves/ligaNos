using ligaNos.Data.Entities;
using ligaNos.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public interface IClubRepository : IGenericRepository<Club>
    {
        IQueryable GetClubWithPlayers();

        Club GetClubByName(string na);


        Task<Club> GetClubWithPlayersAsync(int id);

        Task<Player> GetPlayersAsync(int id);

        Task AddPlayersAsync(PlayersViewModel model, string path);

        Task<int> UpdatePlayersAsync(Player clubManager, string path);

        Task<int> DeletePlayersAsync(Player clubManager);

        IEnumerable<SelectListItem> GetComboClubs();   //video 32

        IEnumerable<SelectListItem> GetComboPlayers(int cubManagerId);   //video 32

        Task<Club> GetClubAsync(Player clubManager); //video 32 para receber uma cidade
        Task UpdateAsync(Task<Club> clube);


        Task<string> GetPositionName(int posid);

        //------------ mostrar staff
        Task AddStaffAsync(StaffViewModel model, string path);

        IQueryable GetClubWithStaff();

        Task<Club> GetClubWithStaffAsync(int id);

        Task<Staff> GetStaffAsync(int id);

        Task<int> UpdateStaffAsync(Staff staff, string path);
        Task<int> DeleteStaffAsync(Staff staff);
        Task<string> GetOccupationName(int posid);


    }
}
