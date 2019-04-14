using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using WebApp_OpenIDConnect_DotNet.Models;

namespace WebApp_OpenIDConnect_DotNet.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly BaseRepoContext _dbContext;
        private readonly IGenericRepo _genericRepo;

        public HomeController(BaseRepoContext dbContext, IGenericRepo genericRepo)
        {
            _dbContext = dbContext;
            _genericRepo = genericRepo;
        }

        public async Task<IActionResult> Index()
        {
            var pais = new Paises { Nombre = "ezee", Habitantes = 3 };
            _dbContext.Paises.Add(pais);
            _dbContext.SaveChanges();

            var obj = _dbContext.Paises.FirstOrDefault(x => x.Nombre== "ezee");

            var all = _genericRepo.GetAll();

            var paises = _genericRepo.GetById(obj.Id);

            var identity = (ClaimsIdentity)User.Identity;

            var claims = identity.Claims.First(x => x.Type == "name").Value;

           //HttpContext.Session.SetString("Test", "Ben Rules!");


            return View(new ErrorViewModel{ RequestId= claims });
        }

        public IActionResult About()
        {
                //ejecutar sp que retorna clases
                var paisesId = new SqlParameter("@PaisesId", 1);
                var paises = _dbContext.Paises
                    .FromSql("EXEC GetPaisesById @PaisesId", paisesId)
                    .ToList();

                //ejecutar sp que retorna int
                var pais = new Paises { Nombre = "ezee", Habitantes = 3 };
                var name = new SqlParameter("@PaisesName", pais.Nombre);
                _dbContext.Database.ExecuteSqlCommand("EXEC AddPais @PaisName", name);


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
