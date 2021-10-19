using ligaNos.Data.Entities;
using ligaNos.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public class ClubRepository : GenericRepository<Club>, IClubRepository
    {
        private readonly DataContext _context;
        public ClubRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetClubWithPlayers() 
        {
            return _context.Clubs
            .Include(c => c.Players)
            .OrderBy(c => c.Name);
        }

        public Club GetClubByName(string na )
        {
            return _context.Clubs
            .Where(x=> x.Name == na )
            .FirstOrDefault();
        }

        public async Task AddPlayersAsync(PlayersViewModel model, string path)
        {
            var club = await this.GetClubWithPlayersAsync(model.ClubId);
            if (club == null)
            {
                return;
            }

            club.Players.Add(new Player
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Number = model.Number,
                PositionId = model.PositionId,
                ImageUrl = path,
            }
           );
            _context.Clubs.Update(club);
            await _context.SaveChangesAsync();
        }


        public async Task<int> DeletePlayersAsync(Player player)   
        {
            var club = await _context.Clubs
                .Where(c => c.Players.Any(ci => ci.Id == player.Id))
                .FirstOrDefaultAsync();
            if (club == null)
            {
                return 0;
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return club.Id;
        }



        public async Task<Club> GetClubWithPlayersAsync(int id) 
        {
            return await _context.Clubs
                .Include(c => c.Players)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }


        public async Task<int> UpdatePlayersAsync(Player player, string path)
        {
            var club = await _context.Clubs
                .Where(c => c.Players.Any(ci => ci.Id == player.Id)).FirstOrDefaultAsync();
            if (club == null)
            {
                return 0;
            }

            player.ImageUrl = path;
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            return club.Id;
        }


        public async Task<Player> GetPlayersAsync(int id) //buscar cidade pelo id
        {
            return await _context.Players.FindAsync(id);
        }


        public async Task<Club> GetClubAsync(Player player)
        {
            return await _context.Clubs
                .Where(c => c.Players.Any(ci => ci.Id == player.Id))
                .FirstOrDefaultAsync();
        }

        // -------------------------------    ^ video 31

        IEnumerable<SelectListItem> IClubRepository.GetComboClubs()
        {
            var list = _context.Clubs.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "(Select a country...)",
                Value = "0"
            });

            return list;
        }

        IEnumerable<SelectListItem> IClubRepository.GetComboPlayers(int clubId)
        {
            var club = _context.Clubs.Find(clubId);
            var list = new List<SelectListItem>();
            if (club != null)
            {
                list = _context.Players.Select(c => new SelectListItem
                {
                    Text = c.FullName,
                    Value = c.Id.ToString()

                }).OrderBy(l => l.Text).ToList();


                list.Insert(0, new SelectListItem
                {
                    Text = "(Select a citie...)",
                    Value = "0"
                });

            }

            return list;
        }


        //update club
        public Task UpdateAsync(Task<Club> clube)
        {
            throw new NotImplementedException();
        }


        //------------------------------------ STAFF CONTROLL

        public IQueryable GetClubWithStaff() //todos
        {
            return _context.Clubs
            .Include(c => c.Staffs)
            .OrderBy(c => c.Name);
        }
        public async Task<Club> GetClubWithStaffAsync(int id)
        {
            return await _context.Clubs
                .Include(c => c.Staffs)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<Staff> GetStaffAsync(int id) //buscar cidade pelo id
        {
            return await _context.Staffs.FindAsync(id);
        }

        public async Task AddStaffAsync(StaffViewModel model, string path)
        {
            var club = await this.GetClubWithStaffAsync(model.ClubId);
            if (club == null)
            {
                return;
            }

            club.Staffs.Add(new Staff
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                PostalCode = model.PostalCode,
                TaxNumber = model.TaxNumber,
                OccupationId = model.OccupationId,
                ImageUrl = path,
                //se precisar de cenas ver o  include e cenas algures

            }
           );
            _context.Clubs.Update(club);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetPositionName(int posid)
        {
            var position = await _context.Positions.FindAsync(posid);
            var posName = position.Name.ToString();
            return posName;
        }

        public async Task<int> UpdateStaffAsync(Staff staff, string path)
        {
            var club = await _context.Clubs
               .Where(c => c.Staffs.Any(ci => ci.Id == staff.Id)).FirstOrDefaultAsync();
            if (club == null)
            {
                return 0;
            }
            staff.ImageUrl = path;
            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync();
            return club.Id;
        }

        public async Task<string> GetOccupationName(int posid)
        {
            var occ = await _context.Occupations.FindAsync(posid);
            var occName = occ.Name.ToString();
            return occName;
        }

        public async Task<int> DeleteStaffAsync(Staff staff)
        {
            var club = await _context.Clubs
                .Where(c => c.Staffs.Any(ci => ci.Id == staff.Id))
                .FirstOrDefaultAsync();
            if (club == null)
            {
                return 0;
            }

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return club.Id;
        }
    }
}
