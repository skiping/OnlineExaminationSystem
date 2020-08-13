using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.DAL;
using OnlineExaminationSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExaminationSystem.BLL.Service
{
    public class ExaminationService
    {
        private readonly DataContext _dbContext;

        public ExaminationService(DataContext dbContex)
        {
            _dbContext = dbContex;
        }

        public async Task<QueryResult<List<ExaminationDto>>> GetExaminations(FilterModel filter)
        {
            var rulesQuery = _dbContext.Examinations.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                rulesQuery = rulesQuery.Where(x => x.Title.Contains(filter.Keyword));
            }

            var query = from a in rulesQuery
                        join b in _dbContext.Subjects
                        on a.SubjectId equals b.Id
                        select new ExaminationDto
                        {
                            Id = a.Id,
                            Title = a.Title,
                            SubjectId = a.SubjectId,
                            Subject = b.Title,
                            Score = a.Score,
                            Time = a.Time,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime
                        };

            var total = await query.CountAsync();
            var data = await query
                .OrderBy(x => x.Id)
                .Skip((filter.PageNo - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new QueryResult<List<ExaminationDto>>()
            {
                Total = total,
                Result = data
            };
        }

        public async Task<QueryResult<List<Examination>>> GetAllExaminations()
        {
            var query = _dbContext.Examinations.AsQueryable();

            var total = await query.CountAsync();
            var data = await query.OrderBy(x => x.Id).ToListAsync();

            return new QueryResult<List<Examination>>()
            {
                Total = total,
                Result = data
            };
        }

        public async Task<bool> DeleteExamination(int id)
        {
            var model = await _dbContext.Examinations.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.Examinations.Remove(model);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<int> EditExamination(ExaminationDto dto)
        {
            if (dto.Id > 0)
            {
                var model = await _dbContext.Examinations.FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (model == null) return 0;

                model.Title = dto.Title;
                model.UpdateTime = DateTime.Now;
                model.SubjectId = dto.SubjectId;
                model.Score = dto.Score;
                model.Time = dto.Time;
            }
            else
            {
                var model = new Examination()
                {
                    Title = dto.Title,
                    SubjectId = dto.SubjectId,
                    Score = dto.Score,
                    Time = dto.Time,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                _dbContext.Examinations.Add(model);
            }

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<ExaminationQuestionModel> GetRondomQuestionsByRule(int ruleId)
        {
            var rule = await _dbContext.ExaminationRules.FirstOrDefaultAsync(x => x.Id == ruleId);
            if (rule == null) return null;

            var questions = await _dbContext.Questions.Where(x => x.SubjectId == rule.SubjectId).ToArrayAsync();
            var choices = questions.Where(x => x.TypeId == 1).OrderBy(x => new Guid()).Take(rule.ChoiceCount).ToList();
            var fills = questions.Where(x => x.TypeId == 3).OrderBy(x => new Guid()).Take(rule.FillCount).ToList();
            var judgments = questions.Where(x => x.TypeId == 2).OrderBy(x => new Guid()).Take(rule.JudgmentCount).ToList();
            var subjectives = questions.Where(x => x.TypeId == 4).OrderBy(x => new Guid()).Take(rule.SubjectiveCount).ToList();

            var examination = new ExaminationQuestionModel()
            {
                ExaminationId = rule.Id,
                ChioceQuestions = choices,
                FillQuestions = fills,
                JudgmentQuestions = judgments,
                SubjectiveQuestions = subjectives
            };

            return examination;
        }
    }
}
