using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewAddLogin.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NewAddLogin.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;



namespace NewAddLogin.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Adds;Integrated Security=true;";
        public IActionResult Index()
        {
            var db = new AdDb(_connectionString);
            var vm = new HomePageViewModel
            {

                Ads = db.GetAllAds(),
                IsAuthenticated = User.Identity.IsAuthenticated
            };
            if (User.Identity.IsAuthenticated)
            {
                vm.UserId = db.GetByEmail(User.Identity.Name).Id;
            }
            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var db = new AdDb(_connectionString);
            User user = db.GetByEmail(User.Identity.Name);
            db.AddAd(ad, user.Id);
            return Redirect("/");
        }
       
        [Authorize]
        [HttpPost]
        public IActionResult Delete(int adId)
        {
            var db = new AdDb(_connectionString);
            db.DeleteAdd(adId);
            return Redirect("/");
        }
    }

}

