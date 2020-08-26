using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class FilterModel
    {
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string Keyword { get; set; }
        public int SubjectId { get; set; }
        public int TypeId { get; set; }
        public int UserId { get; set; }
        public int Year { get; set; }  
    }
}
