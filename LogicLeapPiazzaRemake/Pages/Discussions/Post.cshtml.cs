using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using DataAccess;
using Infrastructure.Models;
using LogicLeapPiazzaRemake.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utility;

namespace LogicLeapPiazzaRemake.Pages.Discussions
{
    [Authorize]
    public class PostModel : PageModel
    {
        public readonly UnitOfWork _unitOfWork;
        public UserManager<ApplicationUser> _userManager;
        public IEnumerable<Post> objPostList;
        public IEnumerable<Comment> objCommentList;
        private readonly IEmailSender _emailSender;

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public Post objPost { get; set; }
        public PostAttachment objPostAttachment { get; set; }
        public Enrollment authorId { get; set; }
        public ApplicationUser author { get; set; }
        public Folder folder { get; set; }
        public int status { get; set; }
        public int userId { get; set; }
        public string currentUser { get; set; }
        public int currentUserId { get; set; }
        public bool IsPostSaved { get; set; }




        public PostModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;
            objPostAttachment = new PostAttachment();
        }

        public async Task OnGetAsync()
        {
            objPost = _unitOfWork.Post.GetByID(int.Parse(Id));
            objCommentList = _unitOfWork.Comment.GetAll().ToList(); // Materialize the comment query result
            IsPostSaved = await CheckIfPostIsSaved(Id);


            status = objPost.StatusId;
            var curruser = await _userManager.GetUserAsync(User);
            if (curruser == null)
            {
                // Handle case where user is not found
                return;
            }
            var currentEnrollmentId = _unitOfWork.Enrollment.GetAll(e => e.ApplicationUserId == curruser.Id)
                                                  .Select(e => e.Id)
                                                  .FirstOrDefault();
            if (currentEnrollmentId == null)
            {
                // Handle case where enrollment ID is not found for the current user
                return;
            }

            // Set the enrollment ID to currentUserId
            currentUserId = currentEnrollmentId;

            Console.WriteLine("objPost is populated: " + objPost.PostTitle.ToString());

            // Get the UserId associated with the EnrollmentId in the Post table
            int enrollmentId = objPost.EnrollmentId;
            int folderid = objPost.FolderId;
            authorId = (from Enrollment in _unitOfWork.Enrollment.GetAll()
                        where Enrollment.Id == enrollmentId
                        select Enrollment).FirstOrDefault();
            Console.WriteLine("AuthorID is populated: " + authorId.ApplicationUserId);

            // Get the ApplicationUser (user) associated with the authorId
            string userId = authorId.ApplicationUserId;
            author = (from user in _unitOfWork.ApplicationUser.GetAll()
                      where user.Id == userId
                      select user).FirstOrDefault();
            Console.WriteLine("Author is populated: " + author.FirstName + " " + author.LastName);

            folder = _unitOfWork.Folder.GetByID(folderid);
            string folderName = folder.Name;
            Console.WriteLine("Folder Name: " + folderName);

            objPostAttachment = _unitOfWork.PostAttachment.Get(a => a.PostId == objPost.Id);
            if(objPostAttachment != null)
            {
                ViewData["MyAttachment"] = objPostAttachment.Url;
            }
            

        }

        public async Task<IActionResult> OnPost()
        {
            //if (Request.Form["DeletePost"].ToString() == "true")
            //{
            //    var post = _unitOfWork.Post.Get(p => p.Id == Int32.Parse(Request.Form["PostId"]));
            //    _unitOfWork.Post.Delete(post);
            //    _unitOfWork.Commit();
            //    return Redirect("/Discussions");
            //}

            if (Request.Form["DeleteComment"].ToString() == "true")
            {
                var comment = _unitOfWork.Comment.Get(p => p.Id == Int32.Parse(Request.Form["CommentId"]));
                _unitOfWork.Comment.Delete(comment);
                _unitOfWork.Commit();
                return RedirectToPage("/Discussions/Post", new { postId = Id });
            }

            if (Request.Form["LikeComment"].ToString() == "true")
            {
                //Check to see if user has already liked this comment
                var commentLike = _unitOfWork.CommentAward.Get(c => c.AwardId == 2 && c.ApplicationUserId == _userManager.GetUserId(User) && c.CommentId == Int32.Parse(Request.Form["CommentId"]));
                if (commentLike != null) //Already liked
                {
                    //Unlike the comment
                    _unitOfWork.CommentAward.Delete(commentLike);
                    _unitOfWork.Commit();
                }
                else
                {
                    _unitOfWork.CommentAward.Add(
                        new CommentAward
                        {
                            ApplicationUserId = _userManager.GetUserId(User),
                            AwardId = 2,
                            CommentId = Int32.Parse(Request.Form["CommentId"])
                        });
                    _unitOfWork.Commit();
                }
                return RedirectToPage("/Discussions/Post", new { postId = Id });
            }

            if (Request.Form["EndorseComment"].ToString() == "true")
            {
                //Check to see if user has already liked this comment
                var commentEndorse = _unitOfWork.CommentAward.Get(c => c.AwardId == 1 && c.ApplicationUserId == _userManager.GetUserId(User) && c.CommentId == Int32.Parse(Request.Form["CommentId"]));
                if (commentEndorse != null) //Already liked
                {
                    //Unlike the comment
                    _unitOfWork.CommentAward.Delete(commentEndorse);
                    _unitOfWork.Commit();
                }
                else
                {
                    _unitOfWork.CommentAward.Add(
                        new CommentAward
                        {
                            ApplicationUserId = _userManager.GetUserId(User),
                            AwardId = 1,
                            CommentId = Int32.Parse(Request.Form["CommentId"])
                        });
                    _unitOfWork.Commit();
                }
                return RedirectToPage("/Discussions/Post", new { postId = Id });
            }

            if (Request.Form["LikePost"].ToString() == "true")
            {
                //Check to see if user has already liked this post
                var postLike = _unitOfWork.PostAward.Get(c => c.AwardId == 2 && c.ApplicationUserId == _userManager.GetUserId(User) && c.PostId == Int32.Parse(Request.Form["PostId"]));
                if (postLike != null) //Already liked
                {
                    //Unlike the post
                    _unitOfWork.PostAward.Delete(postLike);
                    _unitOfWork.Commit();
                }
                else
                {
                    _unitOfWork.PostAward.Add(
                        new PostAward
                        {
                            ApplicationUserId = _userManager.GetUserId(User),
                            AwardId = 2,
                            PostId = Int32.Parse(Request.Form["PostId"])
                        });
                    _unitOfWork.Commit();
                }
                return RedirectToPage("/Discussions/Post", new { postId = Id });
            }

            string note = Request.Form["commentNote"];
            Comment newComment = new Comment();
            newComment.Note = note;
            //newComment.EnrollmentId = authorId.Id;
            newComment.PostId = int.Parse(Id);
            _unitOfWork.Comment.Add(newComment);
            _unitOfWork.Commit();
            await EmailOnActivity(newComment.PostId, $"There is a new comment on one of your saved posts! Here is the comment: {newComment.Note}");
            TempData["success"] = "Comment added successfully.";
            return RedirectToPage("/Discussions/Post", new { postId = Id });
        }

        // Handler method for resolving the issue
        public async Task<IActionResult> OnPostResolveIssue(string resolution)
        {
            int newStatus = resolution switch
            {
                "answeredByInstructor" => 4, // Resolved, answered by instructor
                "answeredByStudent" => 3, // Resolved, answered by student
                _ => 0 // Default status
            };

            // Update post status
            UpdatePostStatus(newStatus);
            await EmailOnActivity(int.Parse(Id), $"The status of a saved post has changed! Here is the new status: {newStatus}");
            // Redirect back to the post page
            return RedirectToPage("/Discussions/Post", new { id = Id });
        }

        // Handler method for resolving the issue
        public async Task<IActionResult> OnPostDeletePost()
        {

            // Update post status
            UpdatePostStatus(5);
            await EmailOnActivity(int.Parse(Id), $"The status of a saved post has changed! Post Archived");
            // Redirect back to the post page
            return RedirectToPage("/Discussions/Index");
        }

        private IActionResult UpdatePostStatus(int status)
        {
            var post = _unitOfWork.Post.GetByID(int.Parse(Id));
            if (post != null && status != 5)
            {
                post.StatusId = status;
                _unitOfWork.Post.Update(post);
                _unitOfWork.Commit(); // Save changes to the database
                return RedirectToPage("/Discussions/Post", Id);
            }
            else if (post != null && status == 5) // Check if post exists and status is 5
            {
                post.StatusId = status;
                _unitOfWork.Post.Update(post);
                _unitOfWork.Commit(); // Save changes to the database
                return RedirectToPage("/Discussions/Index"); // Redirect to discussions page
            }
            else
            {
                // Handle the case where the post is not found
                return RedirectToPage("/Discussions/Index"); // Redirect to discussions page
            }
        }


        public async Task<IActionResult> OnPostSavePost()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Check if the saved post already exists
            var existingSavedPost = await _unitOfWork.SavedPost.GetAsync(sp => sp.ApplicationUserId == currentUser.Id && sp.PostId == int.Parse(Id));

            if (existingSavedPost != null)
            {
                // If the saved post already exists, show a toast message and return to the post page
                TempData["error"] = "This post is already saved.";
                return RedirectToPage("/Discussions/Post", Id);
            }

            var savedPost = new SavedPost
            {
                ApplicationUserId = currentUser.Id,
                PostId = int.Parse(Id)
            };

            _unitOfWork.SavedPost.Add(savedPost);
            await _unitOfWork.CommitAsync();
            TempData["success"] = "Post added to your saved posts.";

            // Redirect back to the post page or wherever appropriate
            return RedirectToPage("/Discussions/Post", Id);
        }

        private async Task<bool> CheckIfPostIsSaved(string postId)
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the current user has saved the post
            if (currentUser != null)
            {
                var savedPost = await _unitOfWork.SavedPost
           .GetAsync(sp => sp.ApplicationUserId == currentUser.Id && sp.PostId == int.Parse(postId));



                return savedPost != null;
            }

            return false;
        }

        public async Task EmailOnActivity(int? postid, string message)
        {
            var user = await _userManager.GetUserAsync(User);
            if (_unitOfWork.SavedPost.GetAll().Any(p => p.PostId == postid && p.ApplicationUserId == user.Id))
            {
                if (user.Email != null && user.EmailConfirmed)
                {
                    await _emailSender.SendEmailAsync(user.Email, "There has been activity on a saved post!",
                        $"Here is what has happened: {message}");
                    return;
                }
            }
            return;
        }

        public async Task<IActionResult> OnPostEditPost(int id, string postTitle, string postDesc)
        {
            var post = _unitOfWork.Post.GetByID(id);

            if (post == null)
            {
                // Handle the case where the post is not found
                return NotFound();
            }

            // Update post properties
            post.PostTitle = postTitle;
            post.PostDesc = postDesc;

            // Save changes to the database
            _unitOfWork.Post.Update(post);
            await _unitOfWork.CommitAsync();

            // Hide the modal after form submission
            TempData["ShowEditModal"] = false;

            // Redirect back to the post page or wherever appropriate
            return RedirectToPage("/Discussions/Post", new { id });
        }

    }
}
