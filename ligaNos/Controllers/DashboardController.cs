﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Controllers
{
    [Authorize(Roles = "Admin, TeamManager, GamesManager")]
    public class DashboardController : Controller
    {
        // GET: DashboardController
        public IActionResult Index()
        {
            return View();
        }

    }
}
