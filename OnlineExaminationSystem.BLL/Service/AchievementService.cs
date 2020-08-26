using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.DAL;
using System.Linq;
using OnlineExaminationSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using OnlineExaminationSystem.Utility;
using System.Threading.Tasks;

namespace OnlineExaminationSystem.BLL.Service
{
    public class AchievementService
    {
        private readonly DataContext _dbContext;

        public AchievementService(DataContext dbContex)
        {
            _dbContext = dbContex;
        }

        /// <summary>
        /// 成绩统计
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public async Task<AchievementStatisticsDto> GetAchievementStatistics(int examId)
        {
            var exams = await (from a in _dbContext.User_Examinations
                                    .Where(x => x.ExaminationId == examId && x.Status == (int)PaperStatus.Marked)
                               join b in _dbContext.Examinations
                               on a.ExaminationId equals b.Id
                               join c in _dbContext.Subjects
                               on b.SubjectId equals c.Id
                               select new
                               {
                                   a.ExaminationId,
                                   Examination = b.Title,
                                   b.SubjectId,
                                   Subject = c.Title,
                                   a.Score
                               }).ToListAsync();

            var total = exams.Count();
            var avgScore = (double)exams.Sum(x => x.Score) / (double)total;
            var max = exams.Max(x => x.Score);
            var min = exams.Min(x => x.Score);

            var exam = exams.FirstOrDefault();
            var statistis = new AchievementStatisticsDto()
            {
                ExminationId = exam.ExaminationId,
                Exmination = exam.Examination,
                SubjectId = exam.SubjectId,
                Subject = exam.Subject,
                AvgScore = avgScore,
                MaxScore = max,
                MinScore = min,
                TotalNums = total
            };

            var section = new Dictionary<string, int>();
            section.Add("0-10", exams.Count(x => x.Score >= 0 && x.Score < 10));
            section.Add("10-20", exams.Count(x => x.Score >= 10 && x.Score < 20));
            section.Add("20-30", exams.Count(x => x.Score >= 20 && x.Score < 30));
            section.Add("30-40", exams.Count(x => x.Score >= 30 && x.Score < 40));
            section.Add("40-50", exams.Count(x => x.Score >= 40 && x.Score < 50));
            section.Add("50-60", exams.Count(x => x.Score >= 50 && x.Score < 60));
            section.Add("60-70", exams.Count(x => x.Score >= 60 && x.Score < 70));
            section.Add("70-80", exams.Count(x => x.Score >= 70 && x.Score < 80));
            section.Add("80-90", exams.Count(x => x.Score >= 80 && x.Score < 90));
            section.Add("90-100", exams.Count(x => x.Score >= 90 && x.Score < 100));
            statistis.ScoreSection = section;

            statistis.Questions = await GetQuestionStatistics(examId);
            return statistis;
        }

        /// <summary>
        /// 获取题目错误率统计
        /// </summary>
        /// <returns></returns>
        public async Task<List<QuestionStatisticsDto>> GetQuestionStatistics(int examId)
        {
            //题目
            var questions = await (from a in _dbContext.Questions
                                   join b in _dbContext.Question_Types
                                   on a.TypeId equals b.Id
                                   join c in _dbContext.Examination_Questions.Where(x => x.ExaminationId == examId)
                                   on a.Id equals c.QuestionId
                                   select new { 
                                       a.Id, 
                                       a.Content,
                                       b.Type 
                                   }).ToListAsync();

            //用户考试答卷
            var answers = await (from a in _dbContext.User_Examination_Answers
                                 join b in _dbContext.User_Examinations
                                       .Where(x => x.ExaminationId == examId && x.Status == (int)PaperStatus.Marked)
                                 on a.UserExaminationId equals b.Id
                                 select a).ToListAsync();

            var statistics = new List<QuestionStatisticsDto>();
            questions.ForEach(x => {

                var wrongCount = answers.Count(y => !y.IsCorrect && y.QuestionId == x.Id);
                var count = answers.Count(y => y.QuestionId == x.Id);
                var rate = count == 0 ? 0 : (double)wrongCount / (double)count;

                var dto = new QuestionStatisticsDto()
                {
                    Id = x.Id,
                    Content = HtmlHelper.RemoveHtmlTag(x.Content),
                    Type = x.Type,
                    WrongRate = rate
                };
                statistics.Add(dto);
            });

            return statistics.OrderByDescending(x => x.WrongRate).ToList();
        }

        /// <summary>
        /// 获取当前登陆用户权限内的所有成绩
        /// 管理员可查看所有科目成绩，老师可查看自己科目的成绩
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="loginUserId"></param>
        public async Task<QueryResult<List<AchievementDto>>> GetUserAchievement(FilterModel filter, int loginUserId)
        {
            var loginUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == loginUserId);

            var query = from a in _dbContext.User_Examinations.Where(x => x.Status == 1) //status = 1已批改的试卷
                        join b in _dbContext.Examinations
                        on a.ExaminationId equals b.Id
                        join c in _dbContext.Subjects
                        on b.SubjectId equals c.Id
                        join d in _dbContext.Users
                        on a.UserId equals d.Id
                        select new AchievementDto
                        {
                            UserId = a.UserId,
                            UserName = d.Name,
                            EmployeeNo = d.EmployeeNo,
                            SubjectId = b.SubjectId,
                            Subject = c.Title,
                            Examination = b.Title,
                            TestTime = a.CreateTime,
                            Score = a.Score,
                        };
            if (loginUser.RoleId == (int)UserRole.Teacher)
            {
                var subjectIds = await _dbContext.User_Subjects
                    .Where(x => x.UserId == loginUserId)
                    .Select(x => x.SubjectId).ToListAsync();

                query = query.Where(x => subjectIds.Contains(x.SubjectId));
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(x => x.Subject.Contains(filter.Keyword) ||
                                         x.Examination.Contains(filter.Keyword) ||
                                         x.UserName.Contains(filter.Keyword));
            }
            if (filter.SubjectId > 0)
            {
                query = query.Where(x => x.SubjectId == filter.SubjectId);
            }
            if (filter.Year > 0)
            {
                query = query.Where(x => x.TestTime.Year == filter.Year);
            }

            var total = await query.CountAsync();
            var data = await query
                .OrderByDescending(x => x.TestTime)
                .Skip((filter.PageNo - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new QueryResult<List<AchievementDto>>()
            {
                Total = total,
                Result = data
            };
        }
    }
}
