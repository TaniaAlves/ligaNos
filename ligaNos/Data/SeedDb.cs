using ligaNos.Data.Entities;
using ligaNos.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            //await _context.Database.EnsureCreatedAsync(); //ve se existe base de dados
            await _context.Database.MigrateAsync(); //video 22. pq assim vai correr as migraçoes tb
                                                    //video 19
            await _userHelper.CheckRoleAsync("Admin"); // cria role admin
            await _userHelper.CheckRoleAsync("GamesManager"); 
            await _userHelper.CheckRoleAsync("TeamManager");
            await _userHelper.CheckRoleAsync("Anonym");
            await _userHelper.CheckRoleAsync("Client");

            //if (!_context.Clubs.Any())              //video 30, para arrancar com cidades também
            //{                                               //atençao temos de criar cidade no user tb! 
            //                                                //var cities = new List<City>();
            //                                                //cities.Add(new City { Name = "Lisboa" });
            //                                                //cities.Add(new City { Name = "Porto" });
            //                                                //cities.Add(new City { Name = "Faro" });

            //    _context.Clubs.Add(new Club
            //    {
            //        //Cities = cities,
            //        Name = "Benfica",
            //        Stadium = "Estádio da Luz",
            //        Address = "Rua B",
            //        PostalCode = "1456-001",
            //        Email = "benf@gmail.com"
            //    });

            //    await _context.SaveChangesAsync();


            //    var user2 = await _userHelper.GetUserByEmailAsync("edusporting@yopmail.com");
            //    if (user2 == null)
            //    {
            //        user2 = new User
            //        {
            //            FirstName = "Eduardo",
            //            LastName = "Maria",
            //            Email = "edusporting@yopmail.com",
            //            UserName = "edusporting@yopmail.com",
            //            PhoneNumber = "212343555",
            //            Address = "Rua E",
            //            PostalCode = "2330-026",
            //            TaxNumber = "123456789",
            //        };

            //        var result = await _userHelper.AddUserAsync(user2, "123456");
            //        if (result != IdentityResult.Success)
            //        {
            //            throw new InvalidOperationException("Could not create the user in seeder");
            //        }

            //        await _userHelper.AddUserToRoleAsync(user2, "TeamManager"); //video 19 a criar um role de admin a este user
            //        var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user2);   //video 33
            //        await _userHelper.ConfirmEmailAsync(user2, token);  //video 33
            //    }

            //    var isInRole2 = await _userHelper.IsUserInRoleAsync(user2, "TeamManager"); //verificar se o admin tem um role
            //    if (!isInRole2)
            //    {
            //        await _userHelper.AddUserToRoleAsync(user2, "TeamManager"); //se nao existir cria
            //    }


                
            //}



            //------------------------------------------------------------------------------admin
            var user = await _userHelper.GetUserByEmailAsync("liganosemail@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Tania",
                    LastName = "Alves",
                    Email = "liganosemail@gmail.com",
                    UserName = "liganosemail@gmail.com",
                    PhoneNumber = "212343555",
                    Address = "Rua Jau 33",
                    PostalCode = "2700-026",
                    TaxNumber = "123456789",
                    ImageUrl ="",
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin"); //video 19 a criar um role de admin a este user
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);   //video 33
                await _userHelper.ConfirmEmailAsync(user, token);  //video 33
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin"); //verificar se o admin tem um role
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin"); //se nao existir cria
            }

            //-----------------------------------------------------------Employee
            //var user2 = await _userHelper.GetUserByEmailAsync("erickofficial@yopmail.com");
            //if (user == null)
            //{
            //    user = new User
            //    {
            //        FirstName = "Erick",
            //        LastName = "Avelã",
            //        Email = "erickofficial@yopmail.com",
            //        UserName = "erickofficial@yopmail.com",
            //        PhoneNumber = "212343555",
            //        Address = "Rua Jau 33",
            //        PostalCode = "2700-026",
            //        TaxNumber = "123456789",
            //        ImageUrl = "",
                   
            //    };

            //    var result = await _userHelper.AddUserAsync(user, "123456");
            //    if (result != IdentityResult.Success)
            //    {
            //        throw new InvalidOperationException("Could not create the user in seeder");
            //    }

            //    await _userHelper.AddUserToRoleAsync(user, "Admin"); //video 19 a criar um role de admin a este user
            //    var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);   //video 33
            //    await _userHelper.ConfirmEmailAsync(user, token);  //video 33
            //}

            //var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin"); //verificar se o admin tem um role
            //if (!isInRole)
            //{
            //    await _userHelper.AddUserToRoleAsync(user, "Admin"); //se nao existir cria
            //}

            //------------------------------------------------------------ Club Managers



            //--------------------------------------------------------positions 

            if (!_context.Positions.Any())
            {
                AddProduct("Advanced");
                AddProduct("Goalkeeper");
                AddProduct("Defense");
                AddProduct("Medium");
                await _context.SaveChangesAsync();
            }

            if (!_context.Occupations.Any())
            {
                AddOccupation("Nurse");
                AddOccupation("Doctor");
                AddOccupation("Coach");
                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name)
        {
            _context.Positions.Add(new Position
            {
                Name = name,
            });
        }

        private void AddOccupation(string name)
        {
            _context.Occupations.Add(new Occupation
            {
                Name = name,
            });
        }
    }
}

