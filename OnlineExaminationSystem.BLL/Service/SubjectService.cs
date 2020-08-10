using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.DAL;
using OnlineExaminationSystem.DAL.Entity;
using System;

namespace OnlineExaminationSystem.BLL.Service
{
    public class SubjectService
    {
        private readonly DataContext _dbContext;

        public SubjectService(DataContext dbContex)
        {
            _dbContext = dbContex;
        }

        public async Task<QueryResult<List<Subject>>> GetSubjects(FilterModel filter)
        {
            var query = _dbContext.Subjects.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(x => x.Title.Contains(filter.Keyword) || x.Description.Contains(filter.Keyword));
            }

            var total = await query.CountAsync();
            var subjects = await query
                .OrderBy(x => x.Id)                          
                .Skip((filter.PageNo - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new QueryResult<List<Subject>>()
            {
                Total = total,
                Result = subjects
            };
        }

        public async Task<bool> DeleteSubject(int id)
        {
            var model = await _dbContext.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.Subjects.Remove(model);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<int> EditSubject(int id, string title, string description)
        {
            if (id > 0)
            {
                var model = await _dbContext.Subjects.FirstOrDefaultAsync(x => x.Id == id);
                if (model == null) return 0;

                model.Title = title;
                model.UpdateTime = DateTime.Now;
                model.Description = description;
            }
            else
            {
                var subject = new Subject()
                {
                    Title = title,
                    Description = description,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                _dbContext.Subjects.Add(subject);
            }

            return await _dbContext.SaveChangesAsync();
        }
    }
}
