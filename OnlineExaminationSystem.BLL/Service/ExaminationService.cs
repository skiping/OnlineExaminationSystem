using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.DAL;
using OnlineExaminationSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// 根据科目查询考试
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public async Task<List<Examination>> GetExaminationsBySubject(int subjectId)
        {
            var exams = await _dbContext.Examinations.Where(x => x.SubjectId == subjectId).ToListAsync();
            return exams;
        }

        /// <summary>
        /// 查询考试
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 查询用户所在科目下的所有考试
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 查询所有考试
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 删除考试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteExamination(int id)
        {
            var model = await _dbContext.Examinations.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.Examinations.Remove(model);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 检查用户是否已经参加过该考试
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> CheckUserExamination(int id, int userId)
        {
            var hasExam = await _dbContext.User_Examinations.AnyAsync(x => x.ExaminationId == id && x.UserId == userId);
            if (hasExam) return false;

            var canExam = await (from a in _dbContext.Examinations.Where(x => x.Id == id)
                                 join b in _dbContext.User_Subjects.Where(x => x.UserId == userId)
                                 on a.SubjectId equals b.SubjectId
                                 select a.Id).AnyAsync();

            return canExam;
        }

        /// <summary>
        /// 手动选择题目，添加考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据规则自动生成考试
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="title"></param>
        /// <param name="questionIds"></param>
        /// <returns></returns>
        public async Task<int> AddAutoExamination(int ruleId, string title, List<int> questionIds)
        {
            var rule = await _dbContext.ExaminationRules.FirstOrDefaultAsync(x => x.Id == ruleId);
            if (rule == null) return 0;

            var score = await _dbContext.Questions.Where(x => questionIds.Contains(x.Id)).SumAsync(x => x.Score);
            var model = new Examination()
            {
                Title = title,
                SubjectId = rule.SubjectId,
                Score = score,
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

        /// <summary>
        /// 编辑考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据试卷规则，随机选择题目生成试卷
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public async Task<ExaminationQuestionModel> GetRondomQuestionsByRule(int ruleId)
        {
            var rule = await _dbContext.ExaminationRules.FirstOrDefaultAsync(x => x.Id == ruleId);
            if (rule == null) return null;

            var questions = (await _dbContext.Questions.Where(x => x.SubjectId == rule.SubjectId).ToListAsync())
                .Select(x => new QuestionDto(x, "", "")).ToList();
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

        /// <summary>
        /// 根据考试ID获取试卷试题
        /// </summary>
        /// <param name="examinationId">考试Id</param>
        /// <returns></returns>
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

            var questions = (await (from a in _dbContext.Examination_Questions
                                    join b in _dbContext.Questions
                                    on a.QuestionId equals b.Id
                                    where a.ExaminationId == examination.Id
                                    select b).ToListAsync()).Select(x => new QuestionDto(x, "", "")).ToList();
            questions.ForEach(x => x.Answer = "");

            //将题型分类，并分别返回
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
                    var iscorrect = answer.Trim().ToLower() == x.Answer.Trim().ToLower();
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

            //如果不包含主观题，则将试卷标为已批改
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

        /// <summary>
        /// 查询用户所有考试成绩
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <returns></returns>
        public async Task<QueryResult<List<UserExaminationDto>>> GetExamResult(FilterModel filter)
        {
            var query = from a in _dbContext.User_Examinations
                        join b in _dbContext.Examinations
                        on a.ExaminationId equals b.Id
                        join c in _dbContext.Subjects
                        on b.SubjectId equals c.Id
                        join d in _dbContext.Users
                        on a.UserId equals d.Id
                        select new UserExaminationDto
                        {
                            Id = a.Id,
                            ExaminationId = b.Id,
                            Title = b.Title,
                            SubjectId = c.Id,
                            Subject = c.Title,
                            Status = a.Status,                          
                            Score = a.Score,
                            TestTime = a.CreateTime,
                            UpdateTime = a.UpdateTime,
                            UserId = d.Id,
                            UserName = d.Name,
                            EmployeeNo = d.EmployeeNo
                        };
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(x => x.Title.Contains(filter.Keyword));
            }
            if (filter.SubjectId > 0)
            {
                query = query.Where(x => x.SubjectId == filter.SubjectId);
            }

            var exams = await query.OrderByDescending(x => x.TestTime)
                                .Skip((filter.PageNo - 1) * filter.PageSize)
                                .Take(filter.PageSize)
                                .ToListAsync();
            var total = await query.CountAsync();
            return new QueryResult<List<UserExaminationDto>>()
            {
                Total = total,
                Result = exams
            };
        }

        /// <summary>
        /// 查询用户所有考试成绩
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<QueryResult<List<UserExaminationDto>>> GetExamResult(FilterModel filter, int userId)
        {
            var query = from a in _dbContext.User_Examinations.Where(x => x.UserId == userId)
                        join b in _dbContext.Examinations
                        on a.ExaminationId equals b.Id
                        join c in _dbContext.Subjects
                        on b.SubjectId equals c.Id
                        select new UserExaminationDto
                        {
                            Id = a.Id,
                            ExaminationId = b.Id,
                            Title = b.Title,
                            SubjectId = c.Id,
                            Subject = c.Title,
                            Status = a.Status,
                            Score = a.Score,
                            TestTime = a.CreateTime,
                            UpdateTime = a.UpdateTime
                        };
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(x => x.Title.Contains(filter.Keyword));
            }
            if (filter.SubjectId > 0)
            {
                query = query.Where(x => x.SubjectId == filter.SubjectId);
            }

            var exams = await query.OrderByDescending(x => x.TestTime)
                                .Skip((filter.PageNo - 1) * filter.PageSize)
                                .Take(filter.PageSize)
                                .ToListAsync();
            var total = await query.CountAsync();

            var examIds = exams.Select(x => x.ExaminationId).ToList();

            //取出跟该用户相关的已批改所有试卷成绩
            var allExams = await _dbContext.User_Examinations
                .Where(x => examIds.Contains(x.ExaminationId) && x.Status == 1).ToListAsync();

            //循环计算各项成绩统计
            exams.ForEach(x =>
            {
                var resultPerExams = allExams.Where(y => y.ExaminationId == x.ExaminationId).ToList();
                if (resultPerExams.Count > 0)
                {
                    x.TotalNums = resultPerExams.Count();
                    x.AvgScore = (double)resultPerExams.Sum(y => y.Score) / (double)resultPerExams.Count();
                    x.MaxScore = resultPerExams.Max(y => y.Score);
                    x.MinScore = resultPerExams.Min(y => y.Score);
                    x.Rank = resultPerExams.Count(y => y.Score > x.Score) + 1;
                }
            });

            return new QueryResult<List<UserExaminationDto>>()
            {
                Total = total,
                Result = exams
            };
        }

        /// <summary>
        /// 获取试卷的试题及相应用户考试提交的答案
        /// </summary>
        /// <param name="userExamId">用户考试Id</param>
        /// <returns></returns>
        public async Task<UserExaminationQuestionDto> GetPaperAndAnswer(int userExamId)
        {
            //用户考试基本信息
            var userExam = await (from a in _dbContext.User_Examinations
                                  join b in _dbContext.Examinations
                                  on a.ExaminationId equals b.Id
                                  join c in _dbContext.Subjects
                                  on b.SubjectId equals c.Id
                                  join d in _dbContext.Users
                                  on a.UserId equals d.Id
                                  where a.Id == userExamId
                                  select new UserExaminationQuestionDto
                                  {
                                      UserExamId = a.Id,
                                      UserId = d.Id,
                                      UserName = d.Name,
                                      EmployeeNo = d.EmployeeNo,
                                      ExaminationId = a.ExaminationId,
                                      ExaminaonName = b.Title,
                                      Subject = c.Title
                                  }).FirstOrDefaultAsync();
            if (userExam == null) return null;

            //试卷题目信息
            var questions = await (from a in _dbContext.Examination_Questions
                                   join b in _dbContext.Questions
                                   on a.QuestionId equals b.Id
                                   where a.ExaminationId == userExam.ExaminationId
                                   select b).ToListAsync();
            
            //用户试卷答案信息
            var userAnswers = await _dbContext.User_Examination_Answers
                .Where(x => x.UserExaminationId == userExamId).ToListAsync();
            var userAnswerModels = userAnswers.Select(x => new UserAnswer()
            {
                Id = x.Id,
                Answer = x.Answer,
                IsMark = x.IsMark,
                IsCorrect = x.IsCorrect,
                Score = x.Score,
                Question = questions.FirstOrDefault(y => y.Id == x.QuestionId)
            }).ToList();

            userExam.ChioceQuestions = userAnswerModels.Where(x => x.Question.TypeId == 1).ToList();
            userExam.JudgmentQuestions = userAnswerModels.Where(x => x.Question.TypeId == 2).ToList();
            userExam.FillQuestions = userAnswerModels.Where(x => x.Question.TypeId == 3).ToList();
            userExam.SubjectiveQuestions = userAnswerModels.Where(x => x.Question.TypeId == 4).ToList();

            return userExam;
        }

        /// <summary>
        /// 批改试卷逻辑
        /// </summary>
        /// <param name="dto">试卷数据</param>
        /// <returns></returns>
        public async Task<bool> MarkPaper(MarkPaperDto dto)
        {
            var userExam = await _dbContext.User_Examinations.FirstOrDefaultAsync(x => x.Id == dto.UserExamId);
            if (userExam == null) return false;

            var userAnswers = await _dbContext.User_Examination_Answers.Where(x => x.UserExaminationId == userExam.Id).ToListAsync();
            userAnswers.ForEach(x =>
            {
                var markedAnswer = dto.Questions.FirstOrDefault(y => y.Id == x.Id);

                x.IsMark = markedAnswer.IsMark;
                x.IsCorrect = markedAnswer.IsCorrect;
                x.Score = markedAnswer.Score;           
            });

            userExam.Score = userAnswers.Sum(x => x.Score);
            userExam.Status = 1;
            userExam.UpdateTime = DateTime.Now;

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
