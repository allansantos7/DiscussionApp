using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {

        // Add models/tables here
        public IGenericRepository<ApplicationUser> ApplicationUser { get; }
        public IGenericRepository<AttachmentType> AttachmentType { get; }
        public IGenericRepository<Award> Award { get; }
        public IGenericRepository<Class> Class { get; }
        public IGenericRepository<Comment> Comment { get; }
        public IGenericRepository<CommentAttachment> CommentAttachment { get; }
        public IGenericRepository<CommentAward> CommentAward { get; }
        public IGenericRepository<Enrollment> Enrollment { get; }
        public IGenericRepository<Folder> Folder { get; }
        public IGenericRepository<Post> Post { get; }
        public IGenericRepository<PostAttachment> PostAttachment { get; }
        public IGenericRepository<PostAward> PostAward { get; }
        public IGenericRepository<PostType> PostType { get; }
        public IGenericRepository<Status> Status { get; }
        public IGenericRepository<UserTag> UserTag { get; }
        public IGenericRepository<SavedPost> SavedPost { get; }


        int Commit();

        Task<int> CommitAsync();
    }
}
