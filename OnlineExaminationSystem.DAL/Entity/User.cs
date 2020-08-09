using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExaminationSystem.DAL.Entity
{
    public class User
    {
        public int Id { get; set; }
        public int RoleId { get; set; } //1管理员 2 教师 3 员工
        public string Name { get; set; }
        public string EmployeeNo { get; set; }
        public string ProductionLine { get; set; }
        public string Station { get; set; }
        public string OJTNo { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public int Age { get; set; }
        public int Sex { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
