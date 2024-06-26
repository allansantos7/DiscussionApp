using DataAccess;
using Infrastructure.Models;
using LogicLeapPiazzaRemake.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LogicLeapPiazzaRemake.Pages.Folders
{
    [Authorize]
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        public IEnumerable<Enrollment> objEnrollmentList { get; set; }
        public IEnumerable<Folder> objFolderList { get; set; }
        private readonly IUserSessionService _userSessionService;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public Folder ObjFolder { get; set; }

        [BindProperty]
        public bool IsUpdate { get; set; }

        [BindProperty]
        public int? ParentId { get; set; }

        [BindProperty]
        public int ClassId { get; set; }

        [BindProperty]
        public string Type { get; set; }

        public UpsertModel(UnitOfWork unitOfWork, IUserSessionService userSessionService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userSessionService = userSessionService;
            objEnrollmentList = new List<Enrollment>();
            objFolderList = new List<Folder>();
            ObjFolder = new Folder();
            ParentId = 0;
            ClassId = 0;
            Type = string.Empty;
            IsUpdate = false;
            _userManager = userManager;
        }

        public IActionResult OnGet(int? id)
        {
            if (id != 0 && id != null)
            {
                ObjFolder = _unitOfWork.Folder.GetByID(id);
                objEnrollmentList = _unitOfWork.Enrollment.GetAll(e => e.ClassId == _userSessionService.GetUserSession().ClassId && e.ApplicationUserId == _userSessionService.GetUserSession().UserId, includes: "Class");
                objFolderList = _unitOfWork.Folder.GetAll(f => f.ClassId == _userSessionService.GetUserSession().ClassId);
                IsUpdate = true;
                return Page();
            }
            else
            {
                objEnrollmentList = _unitOfWork.Enrollment.GetAll(e => e.ClassId == _userSessionService.GetUserSession().ClassId && e.ApplicationUserId == _userSessionService.GetUserSession().UserId, includes: "Class");
                objFolderList = _unitOfWork.Folder.GetAll(f => f.ClassId == _userSessionService.GetUserSession().ClassId);
                IsUpdate = false;
                return Page();
            }
        }

        public IActionResult OnGetAddSubfolder(int? parentid, int classid)
        {
            ClassId = classid;
            ParentId = parentid;
            IsUpdate = false;
            return Page();
        }

        public IActionResult OnGetStudent()
        {
            objEnrollmentList = _unitOfWork.Enrollment.GetAll(e => e.ClassId == _userSessionService.GetUserSession().ClassId && e.ApplicationUserId == _userSessionService.GetUserSession().UserId, includes: "Class");
            objFolderList = _unitOfWork.Folder.GetAll(f => f.ClassId == _userSessionService.GetUserSession().ClassId);
            Type = "student";
            IsUpdate = false;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // Support class dropdown
            int classId;
            Int32.TryParse(Request.Form["SelectedClass"], out classId);
            if (classId != 0)
            {
                await _userSessionService.CreateUserSession(User, _userManager, _unitOfWork, classId);
                return Redirect("/Folders/Upsert");
            }

            if (IsUpdate)
            {
                _unitOfWork.Folder.Update(ObjFolder);
                TempData["success"] = "Folder updated successfully.";
            }
            else
            {
                _unitOfWork.Folder.Add(ObjFolder);
                TempData["success"] = "Folder created successfully.";
            }

            _unitOfWork.Commit();

            if (Type == "student")
            {
                return RedirectToPage("../Discussions/Index");
            }
            else
            {
                return RedirectToPage("Index");
            }
        }
    }
}
