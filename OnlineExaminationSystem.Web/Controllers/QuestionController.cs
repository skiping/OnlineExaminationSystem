using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.BLL.Service;

namespace OnlineExaminationSystem.Web.Controllers
{
    public class QuestionController : Controller
    {
        private readonly QuestionService _service;

        public QuestionController(QuestionService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetQuestions(FilterModel filter)
        {
            var subjects = await _service.GetQuestions(filter);
            return Json(new { Status = true, Data = subjects });
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromBody]QuestionDto dto)
        {
            var result = await _service.AddQuestion(dto);
            return Json(new { Status = result > 0 });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var result = await _service.DeleteQuestion(id);
            return Json(new { Status = result });
        }
    }
}