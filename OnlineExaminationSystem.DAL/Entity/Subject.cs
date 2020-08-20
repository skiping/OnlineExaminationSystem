using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.DAL.Entity
{
    public class Subject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string ImgUrl { get; set; }
    }
}
