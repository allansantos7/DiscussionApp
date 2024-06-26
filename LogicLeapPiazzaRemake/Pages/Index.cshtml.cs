using DataAccess;
using Infrastructure.Models;
using LogicLeapPiazzaRemake.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LogicLeapPiazzaRemake.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public readonly IUserSessionService _userSessionService;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //public List<Post> Posts { get; set; }
        [BindProperty]
        public ApplicationUser user { get; set; }
        [BindProperty]
        public IEnumerable<Post> objPostList { get; set; }
        [BindProperty]
        public Post objPost { get; set; }
        public IEnumerable<Comment> objCommentList;
        public int TotalEndorsedComments { get; set; }
        public IEnumerable<Folder> objFolderList;
        public IEnumerable<Enrollment> objUserEnrollmentList;
        public IEnumerable<SavedPost> SavedPosts = new List<SavedPost>();

        public int TotalTags { get; set; }
        public int TotalSavedPosts { get; set; } // Counter for saved posts

        public List<Comment> AwardedComments { get; set; } = new List<Comment>();
        public List<Comment> MentionedComments { get; set; } = new List<Comment>();
        public List<Post> MentionedPosts { get; set; } = new List<Post>();
        public List<Post> objendorsedCommentList { get; set; } = new List<Post>();




        public IndexModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserSessionService userSessionService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _userSessionService = userSessionService;
            objPostList = new List<Post>();
            objFolderList = new List<Folder>();
            objCommentList = new List<Comment>();
            objPost = new Post();

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            _userSessionService.CreateUserSession(User, _userManager, _unitOfWork);

            if (_userSessionService.GetUserSession().ClassId == 0)
            {
                return RedirectToPage("./Wait");
            }

            objPostList = await _unitOfWork.Post.GetAllAsync(p => p.Enrollment.ClassId == _userSessionService.GetUserSession().ClassId, includes: "Enrollment");
            objFolderList = await _unitOfWork.Folder.GetAllAsync();

            var username = User.Identity.Name.Split('@')[0];

            // Fetch all comments from the database
            var allComments = await _unitOfWork.Comment.GetAllAsync();

            // Fetch all posts from the database
            var allPosts = await _unitOfWork.Post.GetAllAsync();

            // Count the total number of mentions the user has in comments
            var totalMentionsInComments = allComments
                .Where(c => c.Note.Contains($"@{username}")) // Assuming mention format is @username
                .ToList();

            // Count the total number of mentions the user has in posts
            var totalMentionsInPosts = allPosts
                .Where(p => p.PostDesc.Contains($"@{username}")) // Assuming mention format is @username
                .ToList();

            // Push mentioned comments into MentionedComments list
            MentionedComments = totalMentionsInComments;

            // Push mentioned posts into MentionedPosts list
            MentionedPosts = totalMentionsInPosts;

            // Calculate the total number of mentions
            TotalTags = totalMentionsInComments.Count + totalMentionsInPosts.Count;


            // Get the user ID of the logged-in user
            var userId = _userManager.GetUserId(User);

            // Fetch comment awards associated with the enrolled user
            var userCommentAwards = await _unitOfWork.CommentAward
                .GetAllAsync(ca => ca.Comment.Enrollment.ApplicationUserId == userId);

            // Get the IDs of comments with awards for the logged-in user
            var commentIdsWithAwards = userCommentAwards.Select(ca => ca.CommentId).ToList();

            //// Fetch comments with awards for the logged-in user
            //AwardedComments = (await _unitOfWork.Comment
            //    .GetAllAsync(c => commentIdsWithAwards.Contains(c.Id))).ToList();

            // Fetch all comments with AwardId = 1
            var endorsedComments = _unitOfWork.CommentAward.GetAll(ca => ca.AwardId == 1);
            var endorsedCommentPosts = endorsedComments.Select(ca => ca.Comment.Post).ToList();


            // Count the number of endorsed comments
            TotalEndorsedComments = endorsedComments.Count();
            objendorsedCommentList = endorsedCommentPosts;


            //Get all saved comments
            if (user != null)
            {
                // Fetch saved posts associated with the user
                SavedPosts = await _unitOfWork.SavedPost
                    .GetAllAsync(sp => sp.ApplicationUserId == user.Id, includes: "Post");
                // Count the number of saved posts
                TotalSavedPosts = SavedPosts.Count();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            //Capture the possible change from class select\
            int classId;
            Int32.TryParse(Request.Form["SelectedClass"], out classId);
            if (classId != 0)
            {
                _userSessionService.CreateUserSession(User, _userManager, _unitOfWork, classId);
            }
            objPostList = await _unitOfWork.Post.GetAllAsync(p => p.Enrollment.ClassId == _userSessionService.GetUserSession().ClassId, includes: "Enrollment");
            objFolderList = await _unitOfWork.Folder.GetAllAsync();

            return Page();
        }

    }
}
