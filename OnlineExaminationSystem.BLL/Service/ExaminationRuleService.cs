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
    public class ExaminationRuleService
    {
        private readonly DataContext _dbContext;

        public ExaminationRuleService(DataContext dbContex)
        {
            _dbContext = dbContex;
        }

        public async Task<QueryResult<List<ExaminationRuleDto>>> GetRules(FilterModel filter)
        {
            var rulesQuery = _dbContext.ExaminationRules.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                rulesQuery = rulesQuery.Where(x => x.Title.Contains(filter.Keyword));
            }

            var query = from a in rulesQuery
                        join b in _dbContext.Subjects
                        on a.SubjectId equals b.Id
                        select new ExaminationRuleDto
                        {
                            Id = a.Id,
                            Title = a.Title,
                            SubjectId = a.SubjectId,
                            Subject = b.Title,
                            Score = a.Score,
                            Time = a.Time,
                            ChoiceCount = a.ChoiceCount,
                            FillCount = a.FillCount,
                            JudgmentCount = a.JudgmentCount,
                            SubjectiveCount = a.SubjectiveCount,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime
                        };

            var total = await query.CountAsync();
            var data = await query
                .OrderBy(x => x.Id)
                .Skip((filter.PageNo - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new QueryResult<List<ExaminationRuleDto>>()
            {
                Total = total,
                Result = data
            };
        }

        public async Task<QueryResult<List<ExaminationRule>>> GetAllRules()
        {
            var query = _dbContext.ExaminationRules.AsQueryable();

            var total = await query.CountAsync();
            var data = await query.OrderBy(x => x.Id).ToListAsync();

            return new QueryResult<List<ExaminationRule>>()
            {
                Total = total,
                Result = data
            };
        }

        public async Task<bool> DeleteRule(int id)
        {
            var model = await _dbContext.ExaminationRules.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.ExaminationRules.Remove(model);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<int> EditRule(ExaminationRuleDto dto)
        {
            if (dto.Id > 0)
            {
                var model = await _dbContext.ExaminationRules.FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (model == null) return 0;

                model.Title = dto.Title;
                model.UpdateTime = DateTime.Now;
                model.SubjectId = dto.SubjectId;
                model.ChoiceCount = dto.ChoiceCount;
                model.FillCount = dto.FillCount;
                model.JudgmentCount = dto.JudgmentCount;
                model.SubjectiveCount = dto.SubjectiveCount;
                model.Score = dto.Score;
                model.Time = dto.Time;
            }
            else
            {
                var model = new ExaminationRule()
                {
                    Title = dto.Title,
                    SubjectId = dto.SubjectId,
                    ChoiceCount = dto.ChoiceCount,
                    FillCount = dto.FillCount,
                    JudgmentCount = dto.JudgmentCount,
                    SubjectiveCount = dto.SubjectiveCount,
                    Score = dto.Score,
                    Time = dto.Time,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                _dbContext.ExaminationRules.Add(model);
            }

            return await _dbContext.SaveChangesAsync();
        }
    }
}
