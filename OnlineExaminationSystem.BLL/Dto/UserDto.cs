using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.BLL.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        //public string RoleName { get; set; }
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
        public string Sex { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        //public string Subject { get; set; }
    }

    public class UserQueryDto : UserDto
    {
        public string RoleName { get; set; }
        public string Subject { get; set; }
        public List<int> SubjectIds { get; set; }
    }
}
