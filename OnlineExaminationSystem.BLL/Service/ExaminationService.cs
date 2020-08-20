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
                            SubjectImg = b.ImgUrl,
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

        public async Task<QueryResult<List<ExaminationDto>>> GetExaminationsByUser(FilterModel filter, int userId)
        {
            var rulesQuery = _dbContext.Examinations.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                rulesQuery = rulesQuery.Where(x => x.Title.Contains(filter.Keyword));
            }

            var query = from a in rulesQuery
                        join b in _dbContext.Subjects
                        on a.SubjectId equals b.Id
                        join c in _dbContext.User_Subjects.Where(x => x.UserId == userId)
                        on b.Id equals c.SubjectId
                        join d in _dbContext.User_Examinations.Where(x => x.UserId == userId)
                        on a.Id equals d.ExaminationId into temp
                        from e in temp.DefaultIfEmpty()                      
                        select new ExaminationDto
                        {
                            Id = a.Id,
                            Title = a.Title,
                            SubjectId = a.SubjectId,
                            Subject = b.Title,
                            SubjectImg = b.ImgUrl,
                            Score = a.Score,
                            Time = a.Time,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime,
                            HasUserExam = e != null
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

        public async Task<bool> CheckUserExamination(int id, int userId)
        {
            var hasExam = await _dbContext.User_Examinations.AnyAsync(x => x.Id == id && x.UserId == userId);
            if (hasExam) return false;

            var canExam = await (from a in _dbContext.Examinations.Where(x => x.Id == id)
                                 join b in _dbContext.User_Subjects.Where(x => x.UserId == userId)
                                 on a.SubjectId equals b.SubjectId
                                 select a.Id).AnyAsync();

            return canExam;
        }

        public async Task<int> AddManualExamination(ExaminationDto dto)
        {
            if (dto.Id > 0) 
            {
                var model = await _dbContext.Examinations.FirstOrDefaultAsync(x => x.Id == dto.Id);
                model.Title = dto.Title;
                model.Time = dto.Time;
                model.Score = dto.Score;
                model.SubjectId = dto.SubjectId;
                model.UpdateTime = DateTime.Now;

                var questions = await _dbContext.Examination_Questions.Where(x => x.ExaminationId == dto.Id).ToListAsync();

                foreach (var quesiondId in dto.QuestionIds)
                {
                    if (!questions.Any(x => x.QuestionId == quesiondId))
                    {
                        var question = new Examination_Question()
                        {
                            ExaminationId = model.Id,
                            QuestionId = quesiondId
                        };
                        _dbContext.Examination_Questions.Add(question);
                    }
                }

                questions.ForEach(x => { 
                    if (!dto.QuestionIds.Contains(x.QuestionId))
                    {
                        _dbContext.Examination_Questions.Remove(x);
                    }
                });
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
                await _dbContext.SaveChangesAsync();

                foreach (var quesiondId in dto.QuestionIds)
                {
                    var question = new Examination_Question()
                    {
                        ExaminationId = model.Id,
                        QuestionId = quesiondId
                    };
                    _dbContext.Examination_Questions.Add(question);
                }
            }

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> AddAutoExamination(int ruleId, string title, List<int> questionIds)
        {
            var rule = await _dbContext.ExaminationRules.FirstOrDefaultAsync(x => x.Id == ruleId);
            if (rule == null) return 0;

            var model = new Examination()
            {
                Title = title,
                SubjectId = rule.SubjectId,
                Score = rule.Score,
                Time = rule.Time,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            _dbContext.Examinations.Add(model);
            await _dbContext.SaveChangesAsync();

            foreach (var quesiondId in questionIds)
            {
                var question = new Examination_Question()
                {
                    ExaminationId = model.Id,
                    QuestionId = quesiondId
                };
                _dbContext.Examination_Questions.Add(question);
            }
            
            return await _dbContext.SaveChangesAsync();
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

        public async Task<ExaminationQuestionModel> GetQuestionsByExamination(int examinationId)
        {
            var examination = await (from a in _dbContext.Examinations.Where(x => x.Id == examinationId)
                                     join b in _dbContext.Subjects
                                     on a.SubjectId equals b.Id
                                     select new ExaminationDto
                                     {
                                         Id = a.Id,
                                         Title = a.Title,
                                         SubjectId = a.SubjectId,
                                         Subject = b.Title,
                                         SubjectImg = b.ImgUrl,
                                         Score = a.Score,
                                         Time = a.Time,
                                         CreateTime = a.CreateTime,
                                         UpdateTime = a.UpdateTime
                                     }).FirstOrDefaultAsync();
            if (examination == null) return null;

            var questions = await (from a in _dbContext.Examination_Questions
                                   join b in _dbContext.Questions
                                   on a.QuestionId equals b.Id
                                   where a.ExaminationId == examination.Id
                                   select b).ToListAsync();
            questions.ForEach(x => x.Answer = "");

            var choices = questions.Where(x => x.TypeId == 1).ToList();
            var fills = questions.Where(x => x.TypeId == 3).ToList();
            var judgments = questions.Where(x => x.TypeId == 2).ToList();
            var subjectives = questions.Where(x => x.TypeId == 4).ToList();

            var model = new ExaminationQuestionModel()
            {
                ExaminationId = examination.Id,
                Examination = examination,
                ChioceQuestions = choices,
                FillQuestions = fills,
                JudgmentQuestions = judgments,
                SubjectiveQuestions = subjectives
            };

            return model;
        }

        /// <summary>
        /// 用户考试逻辑
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="examinationId">考试Id</param>
        /// <param name="dto">用户提交的答案</param>
        /// <returns></returns>
        public async Task<User_Examination> AddUserExam(int userId, int examinationId, List<AnswerDto> dto)
        {
            var model = await _dbContext.Examinations.FirstOrDefaultAsync(x => x.Id == examinationId);
            if (model == null) return null;

            var userExam = new User_Examination()
            {
                ExaminationId = examinationId,
                UserId = userId,
                UpdateTime = DateTime.Now,
                CreateTime = DateTime.Now,
                Status = 0,
            };
            _dbContext.User_Examinations.Add(userExam);
            await _dbContext.SaveChangesAsync();


            var questions = await (from a in _dbContext.Examination_Questions
                                   join b in _dbContext.Questions
                                   on a.QuestionId equals b.Id
                                   where a.ExaminationId == examinationId
                                   select b).ToListAsync();

            //保存用户试卷答案，并批改非主观题的答案
            var autoScore = 0;
            questions.ForEach(x => {

                var answer = dto.FirstOrDefault(y => y.QuestionId == x.Id).Answer;

                if (x.TypeId != 4)
                {
                    var iscorrect = answer.Trim() == x.Answer.Trim();
                    var user_answer = new User_Examination_Answer()
                    {
                        QuestionId = x.Id,
                        Answer = answer,
                        UserExaminationId = userExam.Id,
                        IsMark = true,
                        IsCorrect = iscorrect,
                        Score = iscorrect ? x.Score : 0,
                    };

                    autoScore += user_answer.Score;
                    _dbContext.User_Examination_Answers.Add(user_answer);
                }
                else
                {
                    var user_answer = new User_Examination_Answer()
                    {
                        QuestionId = x.Id,
                        Answer = answer,
                        IsMark = false,
                        UserExaminationId = userExam.Id
                    };
                    _dbContext.User_Examination_Answers.Add(user_answer);
                }
            });

            userExam.AutoScore = autoScore;
            if (!questions.Any(x => x.TypeId == 4))
            {
                userExam.Score = autoScore;
                userExam.Status = 1;
            }

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return userExam;
            }

            return null;
        }
    }
}
