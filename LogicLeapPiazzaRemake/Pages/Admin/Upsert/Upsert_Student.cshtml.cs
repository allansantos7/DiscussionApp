using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;

namespace LogicLeapPiazzaRemake.Pages.Discussions
{
    public class Upsert_StudentModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        // List of Classes
        public IEnumerable<Class> objInstructorClassList
        {
        get; set; }

        // List of Enrollments
        public IEnumerable<Enrollment> objEnrollmentList
        {
        get; set; }
        [BindProperty]
        public IEnumerable<Enrollment> objInstructorEnrollmentList { get; set; }
        [BindProperty]
        public Enrollment objEnrollment { get; set; }
        // List of Users with Role of Student
        public IEnumerable<ApplicationUser> ApplicationUsers
        {
            get; set;
        }
        public List<ApplicationUser> AllStudents
        {
            get; set;
        }
        // List of Students the User selected on the page
        [BindProperty]
        public string SelectedStudents { get; set; }
        public List<ApplicationUser> selectStudents { get; set; }

        public Upsert_StudentModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            objEnrollmentList = new List<Enrollment>();
            objInstructorEnrollmentList = new List<Enrollment>();
            AllStudents = new List<ApplicationUser>();
            SelectedStudents = new string("");
            objEnrollment = new Enrollment();
            selectStudents = new List<ApplicationUser>();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            var user = await _userManager.GetUserAsync(User);

            // Get list of users with Role of Student
            ApplicationUsers = _unitOfWork.ApplicationUser.GetAll().ToList();
            foreach (var aUser in ApplicationUsers)
            {
                var userRole = await _userManager.GetRolesAsync(aUser);
                if (userRole.Any() && userRole[0] == "Student")
                {
                    AllStudents.Add(aUser);
                }
            }

            AllStudents = AllStudents.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();

            // List of Enrollments for the classes the Instructor is currently teaching
            objInstructorEnrollmentList = await _unitOfWork.Enrollment.GetAllAsync(u => u.ApplicationUserId == user.Id && u.Status == EnrollmentStatus.Active, includes: "Class");
            objInstructorEnrollmentList = objInstructorEnrollmentList.OrderBy(u => u.Class.SemesterTerm).ThenBy(c => c.Class.Name);

            // am I in edit mode?
            if (id != 0 && id != null)
            {
                objEnrollment = _unitOfWork.Enrollment.Get(u => u.Id == id, includes: "ApplicationUser,Class");
            }

            if (objEnrollment == null)    // nothing found in DB
            {
                return NotFound();  // build in page
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            // if this is a new student
            if (objEnrollment.Id == 0) 
            {
                if (!ModelState.IsValid)
                {
                    // if submit button is pressed when no student is selected, return page
                    if (SelectedStudents == null)
                    {
                        TempData["error"] += "No Students selected.";
                        return RedirectToPage("./Upsert_Student");
                    }
                    return Page();
                }
                // if no student's are present in list, return page
                if (SelectedStudents.Equals("[]"))
                {
                    TempData["error"] += "No Students selected.";
                    return RedirectToPage("./Upsert_Student");
                }

                var numAdded = 0;

                // Retrieve list of student id(s) from select list input
                SelectedStudents = SelectedStudents.Replace("[\"", "").Replace("\"]", "").Replace("\\", "").Replace("\"", "");
                var studentList = SelectedStudents.Split(',').Distinct();
                foreach (var id in studentList)
                {
                    selectStudents.Add(_unitOfWork.ApplicationUser.Get(u => u.Id == id));
                }

                foreach (var student in selectStudents)
                {
                    // Is Null if no enrollment exists for that student for the current class
                    objEnrollmentList = _unitOfWork.Enrollment.GetAll(u => u.ApplicationUserId == student.Id, includes: "ApplicationUser,Class");
                    var studEnrollment = objEnrollmentList.Where(u => u.ApplicationUserId == student.Id && u.ClassId == objEnrollment.ClassId);

                    // If student isn't already enrolled, Create enrollment for that student
                    if (!studEnrollment.Any())
                    {
                        objEnrollment.ApplicationUserId = student.Id;
                        objEnrollment.Id = 0;
                        objEnrollment.Status = EnrollmentStatus.Active;
                        _unitOfWork.Enrollment.Add(objEnrollment);

                        numAdded++;

                        await _unitOfWork.CommitAsync();
                    }
                    // If student had an enrollment that is currently Inactive, Re-Activate that enrollment for the class
                    else if (studEnrollment.FirstOrDefault().Status == EnrollmentStatus.Inactive)
                    {
                        var renewEnrollment = studEnrollment.FirstOrDefault();
                        renewEnrollment.Status = EnrollmentStatus.Active;
                        renewEnrollment.Created = DateTime.Now;
                        _unitOfWork.Enrollment.Update(renewEnrollment);

                        numAdded++;

                        await _unitOfWork.CommitAsync();
                    }
                    // If student already has an active enrollment to that class
                    else
                    {
                        TempData["error"] += $"{student.FullName} is already enrolled in that {studEnrollment.FirstOrDefault().Class.Name}.\n";
                        continue;
                    }
                }
                if (numAdded > 0)
                {
                    TempData["success"] += $"{numAdded} Student(s) added Successfully. \n";
                }
                else
                {
                    TempData["error"] += "No changes to Students";
                }
            }
            // This is an existing student
            else
            {
                // Edit student - retrieve student from DB
                var student = _unitOfWork.ApplicationUser.Get(u => u.FirstName == objEnrollment.ApplicationUser.FirstName && u.LastName == objEnrollment.ApplicationUser.LastName);

                // Is Null if no enrollment exists for that student for the current class
                objEnrollment.ApplicationUserId = student.Id;
                var enrollmentExists = _unitOfWork.Enrollment.Get(u => u.ApplicationUserId == objEnrollment.ApplicationUserId && u.ClassId == objEnrollment.ClassId, includes: "Class");
                if (enrollmentExists == null)
                {
                    _unitOfWork.Enrollment.Update(objEnrollment);
                    TempData["success"] += "Student updated Successfully.\n";

                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    TempData["error"] += $"{student.FullName} is already enrolled in that {enrollmentExists.Class.Name}.\n";
                }
            }

            return RedirectToPage("../AdminSettings");
        }
    }
}
