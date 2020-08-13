using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class ExaminationRuleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; }
        public int Time { get; set; }
        public int ChoiceCount { get; set; }
        public int FillCount { get; set; }
        public int JudgmentCount { get; set; }
        public int SubjectiveCount { get; set; }
        public int Score { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
