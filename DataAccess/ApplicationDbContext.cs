using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Models;

namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AttachmentType> AttachmentTypes { get; set; } 
        public DbSet<Award> Awards { get; set; } 
        public DbSet<Class> Classes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentAttachment> CommentAttachments { get; set; }
        public DbSet<CommentAward> CommentAwards { get; set; }
        public DbSet<PostAward> PostAwards { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostAttachment> PostAttachments { get; set; }
        public DbSet<PostType> PostTypes { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<SavedPost> SavedPost { get; set; }


    }
}
