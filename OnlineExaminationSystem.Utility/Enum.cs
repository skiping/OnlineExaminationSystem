using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExaminationSystem.Utility
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public enum UserRole
    {
        Admin = 1,
        Teacher = 2,
        User = 3
    }

    /// <summary>
    /// 试卷状态 0-未批改，1-已批改
    /// </summary>
    public enum PaperStatus
    {
        Unmarked = 0,
        Marked = 1
    }
}
