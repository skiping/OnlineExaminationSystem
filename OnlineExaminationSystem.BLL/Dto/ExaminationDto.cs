using OnlineExaminationSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class ExaminationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class ExaminationQuestionModel
    {
        public int ExaminationId { get; set; }
        public List<Question> ChioceQuestions { get; set; }
        public List<Question> FillQuestions { get; set; }
        public List<Question> JudgmentQuestions { get; set; }
        public List<Question> SubjectiveQuestions { get; set; }
    }
}
