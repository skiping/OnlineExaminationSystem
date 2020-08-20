using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly UserService _userService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AdminController(SubjectService subjectService, ExaminationRuleService examinationRuleService,
            IHostingEnvironment hostingEnvironment, UserService userService)
        {
            _subjectService = subjectService;
            _examinationRuleService = examinationRuleService;
            _hostingEnvironment = hostingEnvironment;
            _userService = userService;
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

        [Authorize(Roles = "1")]
        public IActionResult Users()
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
        public async Task<IActionResult> AddSubject(int id, string title, string description, string img)
        {
            var result = await _subjectService.EditSubject(id, title, description, img);
            return Json(new { Status = result > 0 });
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<ActionResult> ImportImg()
        {
            try
            {
                var fileCount = Request.Form.Files.Count;
                if (fileCount == 0) return Json(new { Success = false });

                var file = Request.Form.Files[0];
                var folder = _hostingEnvironment.WebRootPath + "/Import";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var physicalPath = Path.Combine(folder, Path.GetFileName(file.FileName));
                using (FileStream fs = System.IO.File.Create(physicalPath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }

                return Json(new { Success = true, FileName = $"/Import/{Path.GetFileName(file.FileName)}" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false });
            }
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

        public async Task<IActionResult> GetUsers(FilterModel filter)
        {
            var users = await _userService.GetUsers(filter);
            return Json(new { Status = true, Data = users });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            return Json(new { Status = result });
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserDto dto)
        {
            var result = await _userService.EditUser(dto);
            return Json(new { Status = result > 0 });
        }

        [HttpPost]
        public async Task<IActionResult> EditUserSubjects([FromBody] List<int> subjectIds, int userId)
        {
            var result = await _userService.EditUserSubjects(subjectIds, userId);
            return Json(new { Status = result });
        }
    }
}