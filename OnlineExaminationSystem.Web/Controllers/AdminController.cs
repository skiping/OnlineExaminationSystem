using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.BLL.Service;
using OnlineExaminationSystem.Web.Models;

namespace OnlineExaminationSystem.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly SubjectService _subjectService;

        public AdminController(SubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [Authorize(Roles = "1")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "1")]
        public IActionResult Subject()
        {
            return View();
        }

        public async Task<IActionResult> GetSubjects(FilterModel filter)
        {
            var subjects = await _subjectService.GetSubjects(filter);
            return Json(new { Status = true, Data = subjects });
        }

        [HttpPost]
        public async Task<IActionResult> AddSubject(int id, string title, string description)
        {
            var result = await _subjectService.EditSubject(id, title, description);
            return Json(new { Status = result > 0 });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var result = await _subjectService.DeleteSubject(id);
            return Json(new { Status = result });
        }
    }
}