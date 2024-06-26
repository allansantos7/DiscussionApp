using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LogicLeapPiazzaRemake.Pages.Folders
{
    [Authorize(Roles = "Instructor")]
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        public IEnumerable<Folder> objFolderList;
        public Class objClass;
        public Folder objParentFolder;

        public IndexModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            objFolderList = new List<Folder>();
            objClass = new Class();
            objParentFolder = new Folder();
        }

        public void OnGet()
        {
            objFolderList = _unitOfWork.Folder.GetAll(includes: "Class");
        }

        public IActionResult OnGetDelete(int? id)
        {
            Folder folder = _unitOfWork.Folder.GetByID(id);
            folder.Approved = false;
            _unitOfWork.Folder.Update(folder);
            TempData["success"] = "Folder updated successfully.";
            _unitOfWork.Commit();
            objFolderList = _unitOfWork.Folder.GetAll(includes: "Class");
            return Page();
        }
    }
}
