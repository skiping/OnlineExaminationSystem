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
        private readonly ExaminationRuleService _examinationRuleService;

        public AdminController(SubjectService subjectService, ExaminationRuleService examinationRuleService)
        {
            _subjectService = subjectService;
            _examinationRuleService = examinationRuleService;
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

        [Authorize(Roles = "1")]
        public IActionResult ExaminationRule()
        {
            return View();
        }

        public async Task<IActionResult> GetSubjects(FilterModel filter)
        {
            var subjects = await _subjectService.GetSubjects(filter);
            return Json(new { Status = true, Data = subjects });
        }

        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _subjectService.GetAllSubjects();
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

        public async Task<IActionResult> GetExaminationRules(FilterModel filter)
        {
            var data = await _examinationRuleService.GetRules(filter);
            return Json(new { Status = true, Data = data });
        }

        public async Task<IActionResult> GetAllExaminationRules()
        {
            var data = await _examinationRuleService.GetAllRules();
            return Json(new { Status = true, Data = data });
        }

        [HttpPost]
        public async Task<IActionResult> AddExaminationRule([FromBody] ExaminationRuleDto dto)
        {
            var result = await _examinationRuleService.EditRule(dto);
            return Json(new { Status = result > 0 });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteExaminationRule(int id)
        {
            var result = await _examinationRuleService.DeleteRule(id);
            return Json(new { Status = result });
        }
    }
}