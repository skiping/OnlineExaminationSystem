using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.DAL;
using OnlineExaminationSystem.DAL.Entity;
using OnlineExaminationSystem.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExaminationSystem.BLL.Service
{
    public class QuestionService
    {
        private readonly DataContext _dbContext;

        public QuestionService(DataContext dbContex)
        {
            _dbContext = dbContex;
        }

        public async Task<QueryResult<List<QuestionDto>>> GetQuestions(FilterModel filter)
        {
            var questionQuery = _dbContext.Questions.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                questionQuery = questionQuery.Where(x => x.Title.Contains(filter.Keyword) || 
                                                         x.Content.Contains(filter.Keyword));
            }
            if (filter.SubjectId > 0)
            {
                questionQuery = questionQuery.Where(x => x.SubjectId == filter.SubjectId);
            }
            if (filter.TypeId > 0)
            {
                questionQuery = questionQuery.Where(x => x.TypeId == filter.TypeId);
            }

            var query =  from a in questionQuery
                         join b in _dbContext.Subjects
                         on a.SubjectId equals b.Id
                         join c in _dbContext.Question_Types
                         on a.TypeId equals c.Id
                         select new QuestionDto
                         {
                            Id = a.Id,
                            Title = a.Title,
                            Content = a.Content,
                            TypeId = a.TypeId,
                            QuestionType = c.Type,
                            SubjectId = a.SubjectId,
                            Subject = b.Title,
                            OptionA = a.OptionA,
                            OptionB = a.OptionB,
                            OptionC = a.OptionC,
                            OptionD = a.OptionD,
                            Answer = a.Answer,
                            Score = a.Score,
                            DifficultyDegree = a.DifficultyDegree,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime
                        };

            var total = await query.CountAsync();
            var questions = await query
                .OrderByDescending(x => x.UpdateTime)
                .Skip((filter.PageNo - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            questions.ForEach(x => {
                x.CleanContent = HtmlHelper.RemoveHtmlTag(x.Content);
            });

            return new QueryResult<List<QuestionDto>>()
            {
                Total = total,
                Result = questions
            };
        }

        public async Task<int> AddQuestion(QuestionDto dto)
        {
            if (dto.Id == 0)
            {
                var question = new Question()
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    TypeId = dto.TypeId,
                    SubjectId = dto.SubjectId,
                    OptionA = dto.OptionA,
                    OptionB = dto.OptionB,
                    OptionC = dto.OptionC,
                    OptionD = dto.OptionD,
                    Answer = dto.Answer,
                    Score = dto.Score,
                    DifficultyDegree = dto.DifficultyDegree,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                _dbContext.Questions.Add(question);
            }
            else
            {
                var question = _dbContext.Questions.FirstOrDefault(x => x.Id == dto.Id);
                question.Content = dto.Content;
                question.TypeId = dto.TypeId;
                question.SubjectId = dto.SubjectId;
                question.OptionA = dto.OptionA;
                question.OptionB = dto.OptionB;
                question.OptionC = dto.OptionC;
                question.OptionD = dto.OptionD;
                question.Answer = dto.Answer;
                question.Score = dto.Score;
                question.DifficultyDegree = dto.DifficultyDegree;
                question.UpdateTime = DateTime.Now;
            }

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteQuestion(int id)
        {
            var model = await _dbContext.Questions.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.Questions.Remove(model);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<int> AddQuestions(List<Question> questions)
        {
            questions.ForEach(x => {
                _dbContext.Questions.Add(x);
            });

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> ImportQuestions(string filePath)
        {
            using (var fs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var package = new ExcelPackage(fs))
            {
                var questions = new List<Question>();
                var worksheet = package.Workbook.Worksheets[1];

                int rowCount = worksheet.Dimension.Rows;
                if (rowCount < 2) return 0;

                var types = await _dbContext.Question_Types.ToListAsync();
                var subjects = await _dbContext.Subjects.ToListAsync();
                for (var i = 2; i <= rowCount; i ++)
                {
                    var content = worksheet.Cells[i, 1].Value.ToString();
                    var type = worksheet.Cells[i, 2].Value.ToString();
                    var typeId = types.FirstOrDefault(x => x.Type == type)?.Id ?? 0;

                    var subject = worksheet.Cells[i, 3].Value.ToString();
                    var subjectId = subjects.FirstOrDefault(x => x.Title == subject)?.Id ?? 0;
                    
                    var optionA = worksheet.Cells[i, 4].Value.ToString();
                    var optionB = worksheet.Cells[i, 5].Value.ToString();
                    var optionC = worksheet.Cells[i, 6].Value.ToString();
                    var optionD = worksheet.Cells[i, 7].Value.ToString();

                    var answer = worksheet.Cells[i, 8].Value.ToString();
                    int.TryParse(worksheet.Cells[i, 9].Value.ToString(), out int score);
                    int.TryParse(worksheet.Cells[i, 10].Value.ToString(), out int difficult);

                    var question = new Question()
                    {
                        Content = content,
                        TypeId = typeId,
                        SubjectId = subjectId,
                        OptionA = optionA,
                        OptionB = optionB,
                        OptionC = optionC,
                        OptionD = optionD,
                        Answer = answer,
                        Score = score,
                        DifficultyDegree = difficult,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    questions.Add(question);
                };

                return await AddQuestions(questions);
            }
        }
    }
}
