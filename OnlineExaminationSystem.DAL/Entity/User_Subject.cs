using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.DAL.Entity
{
    public class User_Subject
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubjectId { get; set; }
    }
}
