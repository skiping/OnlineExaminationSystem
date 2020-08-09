using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExaminationSystem.DAL.Entity
{
    public class Examination
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SujectId { get; set; }
        public int Time { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
