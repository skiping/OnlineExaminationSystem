using OnlineExaminationSystem.DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using OnlineExaminationSystem.Utility;

namespace OnlineExaminationSystem.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Examination_Question> Examination_Questions { get; set; }
        public DbSet<ExaminationRule> ExaminationRules { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User_Examination> User_Examinations { get; set; }
        public DbSet<User_Examination_Answer> User_Examination_Answers { get; set; }
        public DbSet<Question_Type> Question_Types { get; set; }
        public DbSet<User_Subject> User_Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().Property(x => x.Name).HasMaxLength(50);
            modelBuilder.Entity<User>().Property(x => x.Password).HasMaxLength(50);
            modelBuilder.Entity<User>().Property(x => x.EmployeeNo).HasMaxLength(50);
            modelBuilder.Entity<User>().Property(x => x.Image).HasMaxLength(200);
            modelBuilder.Entity<User>().Property(x => x.ProductionLine).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(x => x.OJTNo).HasMaxLength(50);
            modelBuilder.Entity<User>().Property(x => x.Station).HasMaxLength(50);
            modelBuilder.Entity<User>().Property(x => x.Email).HasMaxLength(50);
            modelBuilder.Entity<User>().Property(x => x.Sex).HasMaxLength(10);

            modelBuilder.Entity<Role>().HasKey(x => x.Id);
            modelBuilder.Entity<Role>().Property(x => x.Name).HasMaxLength(50);
            modelBuilder.Entity<Role>().Property(x => x.Description).HasMaxLength(500);

            modelBuilder.Entity<Question>().HasKey(x => x.Id);
            modelBuilder.Entity<Question>().Property(x => x.Title).HasMaxLength(100);         

            modelBuilder.Entity<Examination>().HasKey(x => x.Id);
            modelBuilder.Entity<Examination>().Property(x => x.Title).HasMaxLength(100);

            modelBuilder.Entity<ExaminationRule>().HasKey(x => x.Id);
            modelBuilder.Entity<ExaminationRule>().Property(x => x.Title).HasMaxLength(100);

            modelBuilder.Entity<Examination_Question>().HasKey(x => x.Id);

            modelBuilder.Entity<User_Examination>().HasKey(x => x.Id);

            modelBuilder.Entity<User_Examination_Answer>().HasKey(x => x.Id);

            modelBuilder.Entity<Subject>().HasKey(x => x.Id);
            modelBuilder.Entity<Subject>().Property(x => x.Title).HasMaxLength(100);
            modelBuilder.Entity<Subject>().Property(x => x.ImgUrl).HasMaxLength(500);

            modelBuilder.Entity<Question_Type>().HasKey(x => x.Id);
            modelBuilder.Entity<Question_Type>().Property(x => x.Type).HasMaxLength(100);

            modelBuilder.Entity<User_Subject>().HasKey(x => x.Id);

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    RoleId = 1,
                    Name = "Admin",
                    Password = Encrypt.MD5Encrypt("123"),
                    CreateTime = DateTime.Now
                },
                new User()
                {
                    Id = 2,
                    RoleId = 2,
                    Name = "李四",
                    Password = Encrypt.MD5Encrypt("123"),
                    CreateTime = DateTime.Now
                },
                new User()
                {
                    Id = 3,
                    RoleId = 3,
                    Name = "王五",
                    Password = Encrypt.MD5Encrypt("123"),
                    CreateTime = DateTime.Now
                });

            modelBuilder.Entity<Role>().HasData(
               new Role()
               {
                   Id = 1,
                   Name = "管理员",
                   Description = "",
                   CreateTime = DateTime.Now,
                   UpdateTime = DateTime.Now
               },
               new Role()
               {
                   Id = 2,
                   Name = "教师",
                   Description = "",
                   CreateTime = DateTime.Now,
                   UpdateTime = DateTime.Now
               },
               new Role()
               {
                   Id = 3,
                   Name = "员工",
                   Description = "",
                   CreateTime = DateTime.Now,
                   UpdateTime = DateTime.Now
               });

            modelBuilder.Entity<Question_Type>().HasData(
              new Question_Type()
              {
                  Id = 1,
                  Type = "选择题"
              },
              new Question_Type()
              {
                  Id = 2,
                  Type = "判断题"
              },
              new Question_Type()
              {
                  Id = 3,
                  Type = "填空题"
              },
              new Question_Type()
              {
                  Id = 4,
                  Type = "问答题"
              });
        }
    }
}
