using OnlineExaminationSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class QuestionDto
    {
        public QuestionDto()
        {
        }

        public QuestionDto(Question question, string subject, string type) 
        {
            Id = question.Id;
            Title = question.Title;
            Content = question.Content;
            TypeId = question.TypeId;
            QuestionType = type;
            SubjectId = question.SubjectId;
            Subject = subject;
            OptionA = question.OptionA;
            OptionB = question.OptionB;
            OptionC = question.OptionC;
            OptionD = question.OptionD;
            Answer = question.Answer;
            Score = question.Score;
            DifficultyDegree = question.DifficultyDegree;
            CreateTime = question.CreateTime;
            UpdateTime = question.UpdateTime;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CleanContent { get; set; }
        public int TypeId { get; set; }
        public string QuestionType { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string Answer { get; set; }
        public int Score { get; set; }
        public int DifficultyDegree { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
