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
        public string SubjectImg { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool HasUserExam { get; set; }

        public List<int> QuestionIds { get; set; }
    }

    public class ExaminationQuestionModel
    {
        public int ExaminationId { get; set; }
        public ExaminationDto Examination { get; set; }
        public List<QuestionDto> ChioceQuestions { get; set; }
        public List<QuestionDto> FillQuestions { get; set; }
        public List<QuestionDto> JudgmentQuestions { get; set; }
        public List<QuestionDto> SubjectiveQuestions { get; set; }
    }
}
