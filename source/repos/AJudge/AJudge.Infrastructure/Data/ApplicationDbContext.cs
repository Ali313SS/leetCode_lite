using AJudge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AJudge.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<ChatBot> ChatBots { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        public DbSet<Submission> Submission { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProblemTag> ProblemTags { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
        public DbSet<UserTeamInvitation> UserTeamInvitations { get; set; }
      
        public DbSet<Vote> Votes { get; set; }


        public DbSet<UserCoaches> UserCoaches { get; set; }
        public DbSet<CoachRequest> CoachRequests { get; set; }
        public DbSet<UserFriend> UserFriend { get; set; }

        //public DbSet<Manager> Managers { get; set; }
        //public DbSet<ContestProblem> ContestProblems { get; set; }
        //public DbSet<ProblemTag> ProblemTags { get; set; }
        // public DbSet<Coach> Coachs { get; set; }
        //public DbSet<UserCoach> UserCoachs { get; set; }
        //public DbSet<UserGroup> UserGroups { get; set; }
        //public DbSet<UserTeam> UserTeams { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Group>().HasOne(x => x.Leader)
                .WithMany(x => x.LeadGroups)
                .HasForeignKey(x => x.LeaderUserId)
                .IsRequired();

            modelBuilder.Entity<Group>().HasMany(x => x.Managers)
                .WithMany(u => u.ManagersGroups)
                .UsingEntity(j => j.ToTable("GroupManagers"));

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Members)
                .WithMany(u => u.MembersGroups)
                .UsingEntity(j => j.ToTable("GroupMembers"));

            //modelBuilder.Entity<RequestTojoinGroup>().HasKey(x => x.Id);
            //modelBuilder.Entity<RequestTojoinGroup>()
            //    .HasOne(x => x.User);
            //modelBuilder.Entity<RequestTojoinGroup>()
            //    .HasOne(x => x.Group);            

            modelBuilder.Entity<RequestTojoinGroup>().HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestTojoinGroup>().HasOne(r => r.Group)
                .WithMany()
                .HasForeignKey(r => r.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Group>().HasMany(x => x.RequestsTojoinGroup)
                .WithMany(u => u.RequestsTojoinGroup)
                .UsingEntity(j => j.ToTable("RequestsTojoinGroup"));

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Voter)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTeam>()
                .HasKey(e => new { e.UserId, e.TeamId });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Teams)
                .WithMany(t => t.Members)
                .UsingEntity<UserTeam>(
                    join => join
                        .HasOne(ut => ut.Team)
                        .WithMany(t => t.UserTeams)
                        .HasForeignKey(ut => ut.TeamId),
                    join => join
                        .HasOne(ut => ut.User)
                        .WithMany(u => u.UserTeams)
                        .HasForeignKey(ut => ut.UserId),
                    join =>
                    {
                        join.HasKey(ut => new { ut.UserId, ut.TeamId });
                        join.ToTable("UserTeams");
                    });

            modelBuilder.Entity<Contest>()
                .HasOne(c => c.Creator)
                .WithMany(u => u.CompeteContests)
                .HasForeignKey(c => c.CreatorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Define the relation for UserFriend
            modelBuilder.Entity<UserFriend>().HasKey(x => new { x.UserId, x.FriendId });

            modelBuilder.Entity<UserFriend>()
                .HasOne(x => x.User)
                .WithMany(x => x.Friends)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserFriend>()
                .HasOne(x => x.Friend)
                .WithMany(x => x.FriendsOf)
                .HasForeignKey(x => x.FriendId)
                .OnDelete(DeleteBehavior.NoAction);

            // Define the relation for UserCoaches
            modelBuilder.Entity<UserCoaches>().HasKey(x => new { x.CoachId, x.UserId });

            modelBuilder.Entity<UserCoaches>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserCoaches)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserCoaches>()
                .HasOne(x => x.Coach)
                .WithMany(x => x.CoachedByhim)
                .HasForeignKey(x => x.CoachId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Problem>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Problems)
                .UsingEntity(j => j.ToTable("ProblemTags"));

            modelBuilder.Entity<Problem>()
                .HasMany(p => p.TestCases)
                .WithOne(tc => tc.Problem)
                .HasForeignKey(tc => tc.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure many-to-many relationship for ProblemTag
            modelBuilder.Entity<ProblemTag>()
                .HasKey(pt => new { pt.ProblemsId, pt.TagsId });
            modelBuilder.Entity<Tag>()
                .HasMany(t => t.Problems)
                .WithMany(u => u.Tags)
                .UsingEntity<ProblemTag>(
                    join => join
                        .HasOne(ut => ut.Problem)
                        .WithMany(t => t.ProblemTags)
                        .HasForeignKey(ut => ut.ProblemsId),
                    join => join
                        .HasOne(ut => ut.Tag)
                        .WithMany(u => u.ProblemTags)
                        .HasForeignKey(ut => ut.TagsId),
                    join =>
                    {
                        join.HasKey(ut => new { ut.TagsId, ut.ProblemsId });
                        join.ToTable("ProblemTag");
                    });

            modelBuilder.Entity<UserTeamInvitation>().HasKey(x => new { x.UserId, x.TeamId });
            modelBuilder.Entity<UserTeamInvitation>().ToTable("UserTeamInvitation");

            modelBuilder.Entity<UserTeamInvitation>()
                .HasOne(x => x.User)
                .WithMany(x => x.Invitations)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTeamInvitation>()
                .HasOne(x => x.Team)
                .WithMany(x => x.Invitations)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore the Coach navigation on CoachRequest to fix the relationship conflict'
           
            //modelBuilder.Entity<CoachRequest>().Ignore(x => x.Coach);

            modelBuilder.Entity<CoachRequest>().HasKey(x => new { x.UserId, x.CoachId });
            modelBuilder.Entity<CoachRequest>().ToTable("CoachRequest");
            modelBuilder.Entity<CoachRequest>()
                .HasOne(x => x.User)
                .WithMany(x => x.CoachRequests)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
 
        }

    }
}