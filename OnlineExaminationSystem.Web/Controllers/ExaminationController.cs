using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.BLL.Service;

namespace OnlineExaminationSystem.Web.Controllers
{
    public class ExaminationController : Controller
    {
        private readonly ExaminationService _examinationService;

        public ExaminationController(ExaminationService examinationService)
        {
            _examinationService = examinationService;
        }

        public IActionResult PaperList()
        {
            return View();
        }

        public async Task<IActionResult> GetExaminations(FilterModel filter)
        {
            var data = await _examinationService.GetExaminations(filter);
            return Json(new { Status = true, Data = data });
        }

        public async Task<IActionResult> GetExaminationsByUser(FilterModel filter)
        {
            var userIdStr = User.Claims.SingleOrDefault(s => s.Type == "UserId").Value;
            int.TryParse(userIdStr, out int userId);
            if (userId == 0) return Json(new { Result = false });

            var data = await _examinationService.GetExaminations(filter);
            return Json(new { Status = true, Data = data });
        }


        public async Task<IActionResult> GetAllExaminations()
        {
            var data = await _examinationService.GetAllExaminations();
            return Json(new { Status = true, Data = data });
        }

        [HttpPost]
        public async Task<IActionResult> AddExamination([FromBody] ExaminationDto dto)
        {
            var result = await _examinationService.EditExamination(dto);
            return Json(new { Status = result > 0 });
        }

        [HttpPost]
        public async Task<IActionResult> AddAutoExamination(int ruleId, string title, [FromBody]List<int> questionIds)
        {
            var result = await _examinationService.AddAutoExamination(ruleId, title, questionIds);
            return Json(new { Status = result > 0 });
        }

        [HttpPost]
        public async Task<IActionResult> AddManualExamination([FromBody] ExaminationDto dto)
        {
            var result = await _examinationService.AddManualExamination(dto);
            return Json(new { Status = result > 0 });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteExamination(int id)
        {
            var result = await _examinationService.DeleteExamination(id);
            return Json(new { Status = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetRondomQuestionsByRule(int ruleId)
        {
            var data = await _examinationService.GetRondomQuestionsByRule(ruleId);
            return Json(new { Status = true, Data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestionsByExamination(int examinationId)
        {
            var data = await _examinationService.GetQuestionsByExamination(examinationId);
            return Json(new { Status = true, Data = data });
        }
    }
}