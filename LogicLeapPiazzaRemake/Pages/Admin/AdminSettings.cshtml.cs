using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace LogicLeapPiazzaRemake.Pages.Admin
{
    [Authorize(Roles = "Instructor")]
    public class AdminSettingsModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public ApplicationUser user { get; set; }
        // List of Students
        public IEnumerable<ApplicationUser> objStudentList;
        // List of Instructor's Classes
        public IEnumerable<Class> objInstructorClassList;
        // List of Topics
        public IEnumerable<Folder> objTopicList;
        // List of Enrollments
        public IEnumerable<Enrollment> objInstructorEnrollmentList;
        public IEnumerable<Enrollment> objStudentEnrollmentList;
        public IEnumerable<Post> objPostList;
        public IEnumerable<Comment> objCommentList;
        public IEnumerable<CommentAward> objCommentAwardList;
        public IEnumerable<UserTag> objUserTagList;
        public IEnumerable<SavedPost> objSavedPostList;
        public string Message { get; set; }

        public AdminSettingsModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            objStudentList = new List<ApplicationUser>();
            objInstructorClassList = new List<Class>();
            objInstructorEnrollmentList = new List<Enrollment>();
            objPostList = new List<Post>();
            objSavedPostList = new List<SavedPost>();
        }
        
        public async Task<IActionResult> OnGetAsync(string message = null)
        {
            Message = message;
            var user = await _userManager.GetUserAsync(User);

            // List of Enrollments, Classes for the classes the Instructor by enrollment
            objInstructorEnrollmentList = await _unitOfWork.Enrollment.GetAllAsync(u => u.ApplicationUserId == user.Id && u.Status == EnrollmentStatus.Active, includes: "Class");
            objInstructorEnrollmentList = objInstructorEnrollmentList.OrderBy(c => c.Class.SemesterTerm).ThenBy(c => c.Class.Name);

            // List of Students enrollments
            objStudentEnrollmentList = await _unitOfWork.Enrollment.GetAllAsync(u => u.ApplicationUserId != user.Id && u.Status == EnrollmentStatus.Active,includes: "Class,ApplicationUser");

            // filter enrollments by the Instructor's classes
            var filteredStudentEnrollmentList = objStudentEnrollmentList.Where(u => objInstructorEnrollmentList.Any(y => y.ClassId == u.ClassId)).OrderBy(u => u.Class.Name).ThenBy(u => u.ApplicationUser.LastName);
            objStudentEnrollmentList = filteredStudentEnrollmentList.OrderBy(c => c.Class.SemesterTerm).ThenBy(c => c.Class.Name).ThenBy(c => c.ApplicationUser.LastName).ThenBy(c => c.ApplicationUser.FirstName);

            // List of Topics/Folders
            objTopicList = _unitOfWork.Folder.GetAll(includes: "Class");
            var filteredTopicList = objTopicList.Where(u => objInstructorEnrollmentList.Any(y => y.ClassId == u.ClassId));
            objTopicList = filteredTopicList;

            // List of Posts for Instructor's classes
            objPostList = await _unitOfWork.Post.GetAllAsync(includes: "Enrollment,Status,PostType,Folder");
            var filteredPostList = objPostList.Where(u => objStudentEnrollmentList.Any(y => y.Id == u.EnrollmentId));
            objPostList = filteredPostList;

            // List of saved posts (emails turned on)
            objSavedPostList = await _unitOfWork.SavedPost.GetAllAsync(includes: "ApplicationUser,Post");

            return Page();
        }

        public void OnPost()
        {
            // Handle form submission
        }
    }
}
