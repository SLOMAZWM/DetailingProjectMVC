﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ProjektLABDetailing.Models.User;
using Microsoft.AspNetCore.Authorization;

namespace ProjektLABDetailing.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly SignInManager<User> _signInManager;

        public ClientController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult ClientUserPanel()
        {
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Client")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult ChangeDataUser()
        {
            return RedirectToAction("ChangeDataUser", "Account" );
        }
        [HttpGet]
        public IActionResult ServiceHistory()
        {
            return View();
        }
        [HttpGet]
        public IActionResult OrderHistory()
        {
            return View();
        }
    }
    
}
