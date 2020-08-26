using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class AchievementDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmployeeNo { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; }
        public string Examination { get; set; }
        public DateTime TestTime { get; set; }
        public int Score { get; set; }
    }
}
