using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.DAL.Entity
{
    public class Examination_Question
    {
        public int Id { get; set; }
        public int ExaminationId { get; set; }
        public int QuestionId { get; set; }
    }
}
