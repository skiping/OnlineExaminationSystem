using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineExaminationSystem.Web.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "1")]
        public IActionResult Index()
        {
            return View();
        }
    }
}