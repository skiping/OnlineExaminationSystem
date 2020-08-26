using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.BLL.Service;

namespace OnlineExaminationSystem.Web.Controllers
{
    [Authorize(Roles = "1, 2")]
    public class AchievementController : Controller
    {
        private readonly AchievementService _service;
        private readonly ExaminationService _examinationService;

        public AchievementController(AchievementService service, ExaminationService examinationService)
        {
            _service = service;
            _examinationService = examinationService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserAchievement()
        {
            return View();
        }

        public async Task<IActionResult> GetExaminationsByUser(FilterModel filter)
        {
            var userIdStr = User.Claims.SingleOrDefault(s => s.Type == "UserId").Value;
            int.TryParse(userIdStr, out int userId);
            if (userId == 0) return Json(new { Status = false });

            var data = await _service.GetUserAchievement(filter, userId);
            return Json(new { Status = true, Data = data });
        }

        public async Task<IActionResult> GetExaminations(int subjectId)
        {
            var data = await _examinationService.GetExaminationsBySubject(subjectId);
            return Json(new { Status = true, Data = data });
        }

        public async Task<IActionResult> GetStatistic(int examId)
        {
            var data = await _service.GetAchievementStatistics(examId);
            return Json(new { Status = true, Data = data });
        }
    }
}