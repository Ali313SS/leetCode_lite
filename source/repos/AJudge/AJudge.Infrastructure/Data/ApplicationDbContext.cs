using AJudge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Infrastructure.Data
{
    public class ApplicationDbContext:DbContext
    {
       
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<ChatBot> ChatBots { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        public DbSet<Submission> Submission { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vote> Votes { get; set; }

        //public DbSet<ContestProblem> ContestProblems { get; set; }
        //public DbSet<ProblemTag> ProblemTags { get; set; }
       // public DbSet<Coach> Coachs { get; set; }
        //public DbSet<UserCoach> UserCoachs { get; set; }
        //public DbSet<UserGroup> UserGroups { get; set; }
        //public DbSet<UserTeam> UserTeams { get; set; }

       

    }
}
