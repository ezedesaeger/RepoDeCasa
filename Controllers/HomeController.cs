﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp_OpenIDConnect_DotNet.Models;

namespace WebApp_OpenIDConnect_DotNet.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var identity = (ClaimsIdentity)User.Identity;

            var claims = identity.Claims.First(x => x.Type == "name").Value;

           HttpContext.Session.SetString("Test", "Ben Rules!");

            return View(new ErrorViewModel{ RequestId= claims });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            var identity = (ClaimsIdentity)User.Identity;
            
            IEnumerable<Claim> claims = identity.Claims;

            var eze = HttpContext.Session.GetString("Test");

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            var identity = (ClaimsIdentity)User.Identity;

            IEnumerable<Claim> claims = identity.Claims;

            var eze = HttpContext.Session.GetString("Test");

            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}