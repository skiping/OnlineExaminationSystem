﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineExaminationSystem.DAL;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OnlineExaminationSystem.DAL.Entity;
using Microsoft.AspNetCore.Authorization;
using OnlineExaminationSystem.Utility;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using OnlineExaminationSystem.BLL.Service;
using OnlineExaminationSystem.Web.Models;
using OnlineExaminationSystem.BLL.Dto;

namespace OnlineExaminationSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;

        public HomeController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "1, 2")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult NoRight()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return Redirect("/Home/Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Login(string name, string password)
        {
            var user = await _userService.Login(name, password);
            if (user == null) return Json(new { result = false });

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, name));
            identity.AddClaim(new Claim("UserId", user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.RoleId.ToString()));
            await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

            return Json(new { result = true, role = user.RoleId });
        }

        [HttpPost]
        public IActionResult Register([FromBody] UserDto dto)
        {
            var result = _userService.Register(dto);

            return Json(new { result = result });
        }

        [HttpPost]
        public IActionResult UpdatePassword(string oldPwd, string newPwd)
        {
            var userIdStr = User.Claims.SingleOrDefault(s => s.Type == "UserId").Value;
            int.TryParse(userIdStr, out int userId);
            if (userId == 0) return Json(new { Status = false });

            var result = _userService.ChangePassword(userId, oldPwd, newPwd);
            return Json(new { Status = result });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
