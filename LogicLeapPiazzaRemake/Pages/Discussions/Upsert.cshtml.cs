
using Microsoft.AspNetCore.Authorization;
using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LogicLeapPiazzaRemake.Pages.Discussions
{
    [Authorize]
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public Post objPost { get; set; }
        [BindProperty]
        public ApplicationUser CurrentUser { get; set; }

        public UpsertModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            objPost = new Post();
        }

        public async void OnGetAsync(string? id)
        {
            if (id != null)
            {
                CurrentUser = await _userManager.FindByIdAsync(id);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Missing Data in Required Fields";
                return Page();
            }

            // Get User Id

            objPost.Enrollment = _unitOfWork.Enrollment.Get(u => u.ApplicationUserId == CurrentUser.Id);
            objPost.EnrollmentId = objPost.Enrollment.Id;

            if (objPost.PostTypeId == 1)
            {
                objPost.PostType.TypeName = "Question";
            }
            else if (objPost.PostTypeId == 2)
            {
                objPost.PostType.TypeName = "Issue";
            }
            else
            {
                objPost.PostType.TypeName = "Announcement";
            }
            if (objPost.StatusId == 1)
            {
                objPost.Status.StatusName = "Open";
            }
            else
            {
                objPost.Status.StatusName = "Closed";
            }
            objPost.FolderId = 1;
            objPost.Updated = DateTime.Now;
            objPost.Created = DateTime.Now;

            _unitOfWork.Post.Add(objPost);
            TempData["success"] = "Post added Successfully";

            _unitOfWork.Commit();
            return RedirectToPage("./Index");
        }
    }
}
