using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.DAL.Entity
{
    public class User_Examination_Answer
    {
        public int Id { get; set; }
        public int UserExaminationId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public bool IsMark { get; set; }
        public bool IsCorrect { get; set; }
        public int Score { get; set; }
    }
}
