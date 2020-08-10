using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class QueryResult<T> 
    {
        public int Total { get; set; }
        public T Result { get; set; }
    }
}
