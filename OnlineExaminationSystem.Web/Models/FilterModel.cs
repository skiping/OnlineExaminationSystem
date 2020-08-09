using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExaminationSystem.Models
{
    public class FilterModel
    {
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string Keyword { get; set; }
          
    }
}
