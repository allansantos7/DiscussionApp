using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LogicLeapPiazzaRemake.Pages.Discussions
{
    public class Upsert_ClassModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public Class objClass { get; set; }
        [BindProperty]
        public ApplicationUser CurrentUser { get; set; }
        [BindProperty]
        public Enrollment objEnrollment { get; set; }

        public IEnumerable<Class> AllClasses
        {
            get; set;
        }
        // List of Students the User selected on the page
        [BindProperty]
        public string SelectedClasses
        {
            get; set;
        }
        public List<Class> selectClasses
        {
            get; set;
        }

        public Upsert_ClassModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            objEnrollment = new Enrollment();
            objClass = new Class();
            AllClasses = new List<Class>();
            SelectedClasses = new string("");
            selectClasses = new List<Class>();
        }

        public async Task<IActionResult> OnGetAsync(int id = 0)
        {

            var user = await _userManager.GetUserAsync(User);

            // I am adding a class
            if (id == 0)
            {
                // Get list of already created Classes
                AllClasses = await _unitOfWork.Class.GetAllAsync();
                AllClasses = AllClasses.OrderBy(c => c.SemesterTerm).ThenBy(c => c.Name);
            }
            // am I in edit mode?
            else
            {
                objClass = _unitOfWork.Class.GetByID(id);
                objEnrollment = _unitOfWork.Enrollment.Get(u => u.ClassId == id && u.ApplicationUserId == user.Id);
                if (objClass == null)    // nothing found in DB
                {
                    return NotFound();  // build in page
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            // if adding a class
            if (objClass.Id == 0)
            {
                if (!ModelState.IsValid)
                {
                    // if submit button is pressed when no class is selected, return page
                    if (SelectedClasses == null && objClass.Name == null)
                    {
                        TempData["error"] += "No Class(es) entered/selected.";
                        return RedirectToPage("./Upsert_Class");
                    }
                }
                // if submit button is pressed when no class is selected, return page
                if (SelectedClasses == null && objClass.Name.Equals("") || objClass.Name == null && SelectedClasses.Equals("[]"))
                {
                    TempData["error"] += "No Class(es) entered/selected.";
                    return RedirectToPage("./Upsert_Class");
                }

                var numAdded = 0;

                // if the user did select to enroll in an already created class from the select list, create an enrollment
                if (SelectedClasses != null && !SelectedClasses.Equals("[]"))
                {
                    // Retrieve list of student id(s) from select list input
                    SelectedClasses = SelectedClasses.Replace("[\"", "").Replace("\"]", "").Replace("\\", "").Replace("\"", "");
                    var classList = SelectedClasses.Split(',').Distinct();
                    foreach (var id in classList)
                    {
                        // convert class id from string to int
                        var cid = int.Parse(id);
                        // Check if enrollment exists for that class
                        var enroll = _unitOfWork.Enrollment.Get(u => u.ClassId == cid && u.ApplicationUserId == user.Id, includes: "Class");
                        // if enrollment doesn't exist, create one
                        if (enroll == null)
                        {
                            // Create an Enrollment to Class in DB for Instructor
                            objEnrollment.Id = 0;
                            objEnrollment.Created = DateTime.Now;
                            objEnrollment.ApplicationUserId = user.Id;
                            objEnrollment.ClassId = cid;
                            objEnrollment.Status = EnrollmentStatus.Active;
                            _unitOfWork.Enrollment.Add(objEnrollment);

                            if (objClass.Name != null && objEnrollment.Class.Name.Equals(objClass.Name)) {
                                objClass = null;
                            }

                            await _unitOfWork.CommitAsync();
                            numAdded++;
                        }
                        // if enrollment does exist, but is inactive, activate it
                        else if (enroll.Status == EnrollmentStatus.Inactive)
                        {
                            enroll.Status = EnrollmentStatus.Active;
                            enroll.Created = DateTime.Now;
                            _unitOfWork.Enrollment.Update(enroll);

                            if (enroll.Class.Name == objClass.Name)
                            {
                                objClass = null;
                            }

                            await _unitOfWork.CommitAsync();
                            numAdded++;
                            TempData["success"] += $"Enrollment reactivated for {enroll.Class.SemesterTerm} - {enroll.Class.Name}.\n";
                        }
                        // if enrollment does exist, and is active, return the page
                        else
                        {
                            TempData["error"] += $"Enrollment exists for {enroll.Class.Name}.\n";
                        }
                    }
                }
                // if the user manually entered a class
                if (objClass != null && objClass.Name != null && objClass.Semester != null && objClass.SemesterYear != null)
                {
                    var classAdded = 0;
                    objClass.Created = DateTime.Now;

                    // Check if class already exists
                    var classId = _unitOfWork.Class.Get(u => u.Name == objClass.Name && u.Semester == objClass.Semester && u.SemesterYear == objClass.SemesterYear);

                    if (classId == null)
                    {
                        // Create Class in DB
                        _unitOfWork.Class.Add(objClass); // not saved yet
                        await _unitOfWork.CommitAsync();

                        classAdded++;
                        // retrieve class id
                        classId = _unitOfWork.Class.Get(u => u.Name == objClass.Name);
                        TempData["success"] += $"{classId.Name} class created.\n";
                    }

                    // Check if enrollment exists for that class
                    var enrollExists = _unitOfWork.Enrollment.Get(u => u.ClassId == classId.Id && u.ApplicationUserId == user.Id, includes: "Class");

                    // if enrollment doesn't exist, create one
                    if (enrollExists == null)
                    {
                        // Create an Enrollment to Class in DB for Instructor
                        objEnrollment.Created = DateTime.Now;
                        objEnrollment.ApplicationUserId = user.Id;
                        objEnrollment.ClassId = classId.Id;
                        objEnrollment.Status = EnrollmentStatus.Active;
                        _unitOfWork.Enrollment.Add(objEnrollment);

                        await _unitOfWork.CommitAsync();
                        numAdded++;
                    }
                    // if enrollment does exist, but is inactive, activate it
                    else if (enrollExists.Status == EnrollmentStatus.Inactive)
                    {
                        enrollExists.Status = EnrollmentStatus.Active;
                        _unitOfWork.Enrollment.Update(enrollExists);

                        await _unitOfWork.CommitAsync();
                        numAdded++;
                        TempData["success"] += $"Enrollment reactivated for {classId.Name}.\n";
                    }
                    // if enrollment does exist, and is active, return the page
                    else
                    {
                        TempData["error"] += $"Already enrolled into {classId.Name}.\n";
                    }
                }

                if (numAdded > 0)
                {
                    TempData["success"] += $"Successfully enrolled into {numAdded} class(es).\n";
                }
                else
                {
                    TempData["error"] += $"No changes to classes.\n";
                }
            }
            // if editing existing class
            else
            {
                var classExists = _unitOfWork.Class.Get(u => u.Id == objClass.Id && u.Name == objClass.Name);
                if (classExists == null)
                {
                    _unitOfWork.Class.Update(objClass);

                    await _unitOfWork.CommitAsync();
                    TempData["success"] += "Class Successfully Updated.";
                }
                else
                {
                    TempData["error"] = $"{classExists.Name} already exists.";
                }
            }
            return RedirectToPage("../AdminSettings");
        }
    }
}
