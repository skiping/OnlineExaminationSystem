using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.DAL.Entity
{
    public class User_Examination
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ExaminationId { get; set; }
        public int Score { get; set; }
        public int AutoScore { get; set; }
        public int ManualScore { get; set; }
        public int Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
