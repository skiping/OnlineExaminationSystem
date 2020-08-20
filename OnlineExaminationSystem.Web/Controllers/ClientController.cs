using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.BLL.Service;

namespace OnlineExaminationSystem.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly ExaminationService _examinationService;

        public ClientController(ExaminationService examinationService)
        {
            _examinationService = examinationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Exam(int id)
        {
            var userIdStr = User.Claims.SingleOrDefault(s => s.Type == "UserId").Value;
            int.TryParse(userIdStr, out int userId);
            if (userId == 0) return Redirect("/Home/Login");

            if (!(await _examinationService.CheckUserExamination(id, userId)))
            {
                return Redirect("/Client/Index");
            }

            ViewBag.Id = id;
            return View();
        }

        public async Task<IActionResult> GetExaminationsByUser(FilterModel filter)
        {
            var userIdStr = User.Claims.SingleOrDefault(s => s.Type == "UserId").Value;
            int.TryParse(userIdStr, out int userId);
            if (userId == 0) return Json(new { Status = false });

            var data = await _examinationService.GetExaminationsByUser(filter, userId);
            return Json(new { Status = true, Data = data });
        }

        public async Task<IActionResult> GetExamination(int id)
        {
            var data = await _examinationService.GetQuestionsByExamination(id);
            return Json(new { Status = true, Data = data });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPaper(int id, [FromBody]List<AnswerDto> answers)
        {
            var userIdStr = User.Claims.SingleOrDefault(s => s.Type == "UserId").Value;
            int.TryParse(userIdStr, out int userId);
            if (userId == 0) return Json(new { Status = false });

            var data = await _examinationService.AddUserExam(userId, id, answers);
            return Json(new { Status = true, Data = data });
        }
    }
}