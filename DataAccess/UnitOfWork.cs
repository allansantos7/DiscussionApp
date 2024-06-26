using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<ApplicationUser> _ApplicationUser;
        public IGenericRepository<AttachmentType> _AttachmentType;
        public IGenericRepository<Award> _Award;
        public IGenericRepository<Class> _Class;
        public IGenericRepository<Comment> _Comment;
        public IGenericRepository<CommentAttachment> _CommentAttachment;
        public IGenericRepository<CommentAward> _CommentAward;
        public IGenericRepository<PostAward> _PostAward;
        public IGenericRepository<Enrollment> _Enrollment;
        public IGenericRepository<Folder> _Folder;
        public IGenericRepository<Post> _Post;
        public IGenericRepository<PostAttachment> _PostAttachment;
        public IGenericRepository<PostType> _PostType;
        public IGenericRepository<Status> _Status;
        public IGenericRepository<UserTag> _UserTag;
        public IGenericRepository<SavedPost> _SavedPost;


        public IGenericRepository<ApplicationUser> ApplicationUser
        {
            get
            {
                if (_ApplicationUser == null)
                {
                    _ApplicationUser = new GenericRepository<ApplicationUser>(_context);
                }
                return _ApplicationUser;
            }
        }

        public IGenericRepository<AttachmentType> AttachmentType
        {
            get
            {
                if (_AttachmentType == null)
                {
                    _AttachmentType = new GenericRepository<AttachmentType>(_context);
                }
                return _AttachmentType;
            }
        }

        public IGenericRepository<Award> Award
        {
            get
            {
                if (_Award == null)
                {
                    _Award = new GenericRepository<Award>(_context);
                }
                return _Award;
            }
        }

        public IGenericRepository<Class> Class
        {
            get
            {
                if (_Class == null)
                {
                    _Class = new GenericRepository<Class>(_context);
                }
                return _Class;
            }
        }

        public IGenericRepository<Comment> Comment
        {
            get
            {
                if (_Comment == null)
                {
                    _Comment = new GenericRepository<Comment>(_context);
                }
                return _Comment;
            }
        }

        public IGenericRepository<CommentAttachment> CommentAttachment
        {
            get
            {
                if (_CommentAttachment == null)
                {
                    _CommentAttachment = new GenericRepository<CommentAttachment>(_context);
                }
                return _CommentAttachment;
            }
        }

        public IGenericRepository<CommentAward> CommentAward
        {
            get
            {
                if (_CommentAward == null)
                {
                    _CommentAward = new GenericRepository<CommentAward>(_context);
                }
                return _CommentAward;
            }
        }

        public IGenericRepository<PostAward> PostAward
        {
            get
            {
                if (_PostAward == null)
                {
                    _PostAward = new GenericRepository<PostAward>(_context);
                }
                return _PostAward;
            }
        }

        public IGenericRepository<Enrollment> Enrollment
        {
            get
            {
                if (_Enrollment == null)
                {
                    _Enrollment = new GenericRepository<Enrollment>(_context);
                }
                return _Enrollment;
            }
        }

        public IGenericRepository<Folder> Folder
        {
            get
            {
                if (_Folder == null)
                {
                    _Folder = new GenericRepository<Folder>(_context);
                }
                return _Folder;
            }
        }

        public IGenericRepository<Post> Post
        {
            get
            {
                if (_Post == null)
                {
                    _Post = new GenericRepository<Post>(_context);
                }
                return _Post;
            }
        }

        public IGenericRepository<PostAttachment> PostAttachment
        {
            get
            {
                if (_PostAttachment == null)
                {
                    _PostAttachment = new GenericRepository<PostAttachment>(_context);
                }
                return _PostAttachment;
            }
        }

        public IGenericRepository<PostType> PostType
        {
            get
            {
                if (_PostType == null)
                {
                    _PostType = new GenericRepository<PostType>(_context);
                }
                return _PostType;
            }
        }

        public IGenericRepository<Status> Status
        {
            get
            {
                if (_Status == null)
                {
                    _Status = new GenericRepository<Status>(_context);
                }
                return _Status;
            }
        }

        public IGenericRepository<UserTag> UserTag
        {
            get
            {
                if (_UserTag == null)
                {
                    _UserTag = new GenericRepository<UserTag>(_context);
                }
                return _UserTag;
            }
        }

        public IGenericRepository<SavedPost> SavedPost
        {
            get
            {
                if (_SavedPost == null)
                {
                    _SavedPost = new GenericRepository<SavedPost>(_context);
                }
                return _SavedPost;
            }
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
