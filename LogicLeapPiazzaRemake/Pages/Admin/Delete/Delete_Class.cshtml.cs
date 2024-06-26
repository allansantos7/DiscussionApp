using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LogicLeapPiazzaRemake.Pages.Discussions
{
    public class Delete_ClassModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public Class objClass { get; set; }

        public IEnumerable<Enrollment> objEnrollmentList { get; set; }

        public Delete_ClassModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            objEnrollmentList = new List<Enrollment>();
            objClass = new Class();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            var user = await _userManager.GetUserAsync(User);

            objClass = _unitOfWork.Class.GetByID(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dropping Class Failed.";
                return Page();
            }
            else
            {
                objEnrollmentList = _unitOfWork.Enrollment.GetAll(u => u.ClassId == objClass.Id, includes: "ApplicationUser");
                foreach (var enrollment in objEnrollmentList)
                {
                    // Class doesn't get deleted, only the enrollment to the class.
                    enrollment.Status = EnrollmentStatus.Inactive;
                    _unitOfWork.Enrollment.Update(enrollment);
                }
            }
            TempData["success"] = "Class Successfully Dropped.";
            _unitOfWork.Commit();

            return RedirectToPage("../AdminSettings");

        }
    }
}
