using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class QuestionStatisticsDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public double WrongRate { get; set; }
    }
}
