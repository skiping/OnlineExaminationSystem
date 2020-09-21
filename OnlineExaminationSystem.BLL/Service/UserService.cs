using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.BLL.Dto;
using OnlineExaminationSystem.DAL;
using OnlineExaminationSystem.DAL.Entity;
using OnlineExaminationSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExaminationSystem.BLL.Service
{
    public class UserService
    {
        private readonly DataContext _dbContext;

        public UserService(DataContext dbContex)
        {
            _dbContext = dbContex;
        }

        public async Task<User> Login(string name, string password)
        {
            var pwd = Encrypt.MD5Encrypt(password);
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Name == name && x.Password == pwd);
            return user;
        }

        public bool ChangePassword(int userId, string oldPassword, string password)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
                if (user == null || user.Password != Encrypt.MD5Encrypt(oldPassword))
                    return false;
                user.Password = Encrypt.MD5Encrypt(password);

                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Register(UserDto dto)
        {
            var user = new User()
            {
                Name = dto.Name,
                RoleId = dto.RoleId,
                Password = Encrypt.MD5Encrypt(dto.Password),
                EmployeeNo = dto.EmployeeNo,
                ProductionLine = dto.ProductionLine,
                Station = dto.Station,
                OJTNo = dto.OJTNo,
                CreateTime = DateTime.Now
            };
            _dbContext.Users.Add(user);
            return _dbContext.SaveChanges() > 0;
        }

        public async Task<QueryResult<List<UserQueryDto>>> GetUsers(FilterModel filter)
        {
            var query = _dbContext.Users.Where(x => x.RoleId > 1).AsQueryable();
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(x => x.Name.Contains(filter.Keyword) || x.EmployeeNo.Contains(filter.Keyword));
            }

            var total = await query.CountAsync();
            var users = await query
                .OrderBy(x => x.Id)
                .Skip((filter.PageNo - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(a => new UserQueryDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    EmployeeNo = a.EmployeeNo,
                    Email = a.Email,
                    Age = a.Age,
                    OJTNo = a.OJTNo,
                    ProductionLine = a.ProductionLine,
                    Phone = a.Phone,
                    RoleId = a.RoleId,
                    Password = a.Password,
                    Image = a.Image,
                    Sex = a.Sex,
                    Station = a.Station,
                    Status = a.Status,
                    CreateTime = a.CreateTime,
                    UpdateTime = a.UpdateTime
                }).ToListAsync();

            var userIds = users.Select(x => x.Id).ToList();
            var roles = await _dbContext.Roles.ToListAsync();
            var subjectList = await (from a in _dbContext.User_Subjects
                                 join b in _dbContext.Subjects
                                 on a.SubjectId equals b.Id
                                 where userIds.Contains(a.UserId)
                                 select new
                                 {
                                     a.UserId,
                                     b.Id,
                                     b.Title
                                 }).ToListAsync();

            users.ForEach(x =>
            {
                x.RoleName = roles.FirstOrDefault(y => y.Id == x.RoleId)?.Name;
                var subjects = subjectList.Where(y => y.UserId == x.Id).ToList();
                x.Subject = string.Join(", ", subjects.Select(x => x.Title).ToList());
                x.SubjectIds = subjects.Select(x => x.Id).ToList();
            });

            return new QueryResult<List<UserQueryDto>>()
            {
                Total = total,
                Result = users
            };
        }

        public async Task<bool> DeleteUser(int id)
        {
            var model = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.Users.Remove(model);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditUserSubjects(List<int> subjectIds, int userId)
        {
            var model = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var subjects = await _dbContext.User_Subjects.Where(x => x.UserId == userId).ToListAsync();

            foreach (var subjectId in subjectIds)
            {
                if (!subjects.Any(x => x.Id == subjectId))
                {
                    var userSubject = new User_Subject()
                    {
                        UserId = userId,
                        SubjectId = subjectId
                    };
                    _dbContext.User_Subjects.Add(userSubject);
                }
            }

            subjects.ForEach(x => {
                if (!subjectIds.Contains(x.Id))
                {
                    _dbContext.User_Subjects.Remove(x);
                }
            });

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<int> EditUser(UserDto dto)
        {
            if (dto.Id > 0)
            {
                var model = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (model == null) return 0;

                model.Name = dto.Name;
                model.UpdateTime = DateTime.Now;
                model.RoleId = dto.RoleId;
                model.EmployeeNo = dto.EmployeeNo;
                model.Age = dto.Age;
                model.Sex = dto.Sex;
                model.OJTNo = dto.OJTNo;
                model.Station = dto.Station;
                model.ProductionLine = dto.ProductionLine;
                model.Phone = dto.Phone;
                model.Status = dto.Status;
                model.Image = dto.Image;
                model.Email = dto.Email;
            }
            else
            {
                var model = new User();
                model.Name = dto.Name;
                model.UpdateTime = DateTime.Now;
                model.RoleId = dto.RoleId;
                model.EmployeeNo = dto.EmployeeNo;
                model.Age = dto.Age;
                model.Sex = dto.Sex;
                model.OJTNo = dto.OJTNo;
                model.Station = dto.Station;
                model.ProductionLine = dto.ProductionLine;
                model.Phone = dto.Phone;
                model.Status = dto.Status;
                model.Image = dto.Image;
                model.Email = dto.Email;
                model.Password = Encrypt.MD5Encrypt("123");

                _dbContext.Users.Add(model);
            }

            return await _dbContext.SaveChangesAsync();
        }
    }
}
