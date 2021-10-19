using ligaNos.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<MatchCalendar> MatchesCalendar { get; set; }
        public DbSet<Occupation> Occupations { get; set; }
        public DbSet<PlayerResult> PlayerResults { get; set; }
        public DbSet<ClubResult> ClubResults { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<ResultTemp> ResultTemps { get; set; }


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}
