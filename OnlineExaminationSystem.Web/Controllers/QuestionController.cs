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

namespace OnlineExaminationSystem.Web.Controllers
{
    [Authorize(Roles = "1, 2")]
    public class QuestionController : Controller
    {
        private readonly QuestionService _service;
        private readonly IHostingEnvironment _hostingEnvironment;

        public QuestionController(QuestionService service, IHostingEnvironment hostingEnvironment)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
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

        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<ActionResult> ImportQuestion()
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

                var result = await _service.ImportQuestions(physicalPath);
                return Json(new { Success = result > 0 });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false });
            }
        }

    }
}