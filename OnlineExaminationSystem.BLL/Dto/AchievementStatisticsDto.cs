using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class AchievementStatisticsDto
    {
        public int ExminationId { get; set; }
        public string Exmination { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; }
        public double AvgScore { get; set; }
        public int TotalNums { get; set; }
        public int MaxScore { get; set; }
        public int MinScore { get; set; }

        public Dictionary<string, int> ScoreSection { get; set; }
        public List<QuestionStatisticsDto> Questions { get; set; }
    }
}
