using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.DAL;
using OnlineExaminationSystem.DAL.Entity;
using OnlineExaminationSystem.Utility;
using System;
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
                if (user == null || user.Password != oldPassword)
                    return false;
                user.Password = password;

                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Register(string username, string password, string phone, int roleId)
        {
            var user = new User()
            {
                Name = username,
                RoleId = roleId,
                Password = Encrypt.MD5Encrypt(password),
                Phone = phone,
                CreateTime = DateTime.Now
            };
            _dbContext.Users.Add(user);
            return _dbContext.SaveChanges() > 0;
        }
    }
}
