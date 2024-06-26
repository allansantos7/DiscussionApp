using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LogicLeapPiazzaRemake.Pages.Discussions
{
    public class Delete_StudentModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public Enrollment studentEnrollment { get; set; }

        public Delete_StudentModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            studentEnrollment = new Enrollment();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            var user = await _userManager.GetUserAsync(User);

            studentEnrollment = _unitOfWork.Enrollment.Get(u => u.Id == id && u.Status == EnrollmentStatus.Active, includes: "ApplicationUser,Class");

            if (studentEnrollment == null)
            {
                return NotFound();
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Data Error unable to connect to DB";
                return Page();
            }
            else
            {
                // delete all student's enrollment to the specifically selected Instructor's class
                var enroll = _unitOfWork.Enrollment.Get(u => u.Id == studentEnrollment.Id);
                studentEnrollment.ApplicationUserId = enroll.ApplicationUserId;
                studentEnrollment.ApplicationUser = enroll.ApplicationUser;
                studentEnrollment.ClassId = enroll.ClassId;
                studentEnrollment.Status = EnrollmentStatus.Inactive;
                _unitOfWork.Enrollment.Update(studentEnrollment);
                
                _unitOfWork.Commit();
                TempData["success"] = "Student Successfully Dropped.";
                return RedirectToPage("../AdminSettings");

            }
        }
    }
}
