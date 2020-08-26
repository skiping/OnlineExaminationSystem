using OnlineExaminationSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class UserExaminationDto
    {
        public int Id { get; set; }
        public int ExaminationId { get; set; }
        public string Title { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; }
        public DateTime TestTime { get; set; }
        public int Status { get; set; }
        public int Score { get; set; }
        public double AvgScore { get; set; }
        public int Rank { get; set; }
        public int TotalNums { get; set; }
        public int MaxScore { get; set; }
        public int MinScore { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmployeeNo { get; set; }
    }

    public class UserAnswer
    {
        public int Id { get; set; }
        public string Answer { get; set; }
        public bool IsMark { get; set; }
        public bool IsCorrect { get; set; }
        public int Score { get; set; }
        public Question Question { get; set; }
    }

    public class UserExaminationQuestionDto
    {
        public int UserExamId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmployeeNo { get; set; }
        public int ExaminationId { get; set; }
        public string ExaminaonName { get; set; }
        public string Subject { get; set; }
        public List<UserAnswer> ChioceQuestions { get; set; }
        public List<UserAnswer> FillQuestions { get; set; }
        public List<UserAnswer> JudgmentQuestions { get; set; }
        public List<UserAnswer> SubjectiveQuestions { get; set; }
    }

    public class MarkPaperDto
    {
        public int UserExamId { get; set; }
        public List<UserAnswer> Questions { get; set; }
    }
}
