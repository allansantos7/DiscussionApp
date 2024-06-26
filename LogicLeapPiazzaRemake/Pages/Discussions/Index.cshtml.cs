using System;
using System.Collections.Generic;
using DataAccess;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using LogicLeapPiazzaRemake.Services;
using System.Net.Mail;

namespace LogicLeapPiazzaRemake.Pages.Discussions
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserSessionService _userSessionService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //public List<Post> Posts { get; set; }
        [BindProperty]
        public ApplicationUser user { get; set; }
        [BindProperty]
        public IEnumerable<Post> objPostList { get; set; }
        [BindProperty]
        public Post objPost { get; set; }
        public PostAttachment objAttachment { get; set; }
        public IEnumerable<Comment> objCommentList;

        public IEnumerable<Folder> objFolderList;

        public IEnumerable<Folder> AllFolders;

        public string SearchBoxContent { get; set; }
        public int currentUserId { get; set; }
        public Enrollment currentEnrollment;
        public IEnumerable<Status> objStatusList;

        public IndexModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserSessionService userSessionService, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            objPostList = new List<Post>();
            objFolderList = new List<Folder>();
            objCommentList = new List<Comment>();
            objStatusList = new List<Status>();
            objPost = new Post();
            SearchBoxContent = string.Empty;
            _userSessionService = userSessionService;
            AllFolders = new List<Folder>();
            objAttachment = new PostAttachment();
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_userSessionService.GetUserSession().ClassId == 0)
            {
                return RedirectToPage("../Wait");
            }
            var user = await _userManager.GetUserAsync(User);
            objPostList = await _unitOfWork.Post.GetAllAsync(p => p.Enrollment.ClassId == _userSessionService.GetUserSession().ClassId, includes:"Folder");
            objFolderList = await _unitOfWork.Folder.GetAllAsync(f => f.ClassId == _userSessionService.GetUserSession().ClassId/* && f.Approved == true*/);
            objStatusList = await _unitOfWork.Status.GetAllAsync();
            AllFolders = await _unitOfWork.Folder.GetAllAsync(f => f.ClassId == _userSessionService.GetUserSession().ClassId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Capture the possible change from class select\
            int classId;
            Int32.TryParse(Request.Form["SelectedClass"], out classId);
            if (classId != 0)
            {
                await _userSessionService.CreateUserSession(User, _userManager, _unitOfWork, classId);
                return Redirect("/Discussions");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Post creation failed.";
                return Page();
            }

            var Enrollment = await _unitOfWork.Enrollment.GetAsync(u => u.ClassId == _userSessionService.GetUserSession().ClassId && u.ApplicationUserId == _userSessionService.GetUserSession().UserId);
            var folderId = Request.Form["objPost.FolderId"]; // Retrieve the selected folder ID from the form data
                                                             // Check if the folderId is a valid integer
            if (int.TryParse(folderId, out int folderIdInt))
            {
                // Update the FolderId of objPost
                objPost.FolderId = folderIdInt;
            }
            else
            {
                // Handle invalid folderId
                TempData["error"] = "Invalid folder selection.";
                return Page();
            }

            var Folder = await _unitOfWork.Folder.GetAllAsync();

            // Update once class selection is available
            objPost.EnrollmentId = Enrollment.Id;
            objPost.Updated = DateTime.Now;

            objPost.Created = DateTime.Now;

            _unitOfWork.Post.Add(objPost);

            _unitOfWork.Commit();

            //1image 2file 3video
            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            if(files.Count > 0)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(webRootPath, @"attachments\images\");
                var extension = Path.GetExtension(files[0].FileName);
                var fullPath = uploads + fileName + extension;

                using var fileStream = System.IO.File.Create(fullPath);
                files[0].CopyTo(fileStream);

                objAttachment.Url = @"\attachments\images\" + fileName + extension;
            }



            objAttachment.PostId = objPost.Id;
            
            objAttachment.AttachmentTypeId = 1;
            _unitOfWork.PostAttachment.Add(objAttachment);

            _unitOfWork.Commit();

            objPost = new Post();
            TempData["success"] = "Post created successfully.";
            objPostList = _unitOfWork.Post.GetAll(p => p.Enrollment.ClassId == _userSessionService.GetUserSession().ClassId).ToList();
            objFolderList = _unitOfWork.Folder.GetAll(f => f.ClassId == _userSessionService.GetUserSession().ClassId && f.Approved == true);
            objStatusList = await _unitOfWork.Status.GetAllAsync();
            AllFolders = await _unitOfWork.Folder.GetAllAsync(f => f.ClassId == _userSessionService.GetUserSession().ClassId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostFilter()
        {
            //Capture the possible change from class select\
            int classId;
            Int32.TryParse(Request.Form["SelectedClass"], out classId);
            if (classId != 0)
            {
                await _userSessionService.CreateUserSession(User, _userManager, _unitOfWork, classId);
                return Redirect("/Discussions");
            }
            objPostList = await _unitOfWork.Post.GetAllAsync(u => u.Enrollment.ClassId == _userSessionService.GetUserSession().ClassId, includes: "Folder");

            // Filter by searchbox
            SearchBoxContent = Request.Form["searchBox"];

            if (SearchBoxContent != "")
            {
                var phrases = new string[5];
                var words = new string[20];

                int runs = 0;
                while (SearchBoxContent.Contains('"') && runs <= 5)
                {
                    int startIndex = SearchBoxContent.IndexOf('"') + 1;
                    int endIndex = SearchBoxContent.IndexOf('"', startIndex);
                    var exact = SearchBoxContent.Substring(startIndex - 1, (endIndex - startIndex) + 2);
                    var search = SearchBoxContent.Substring(startIndex, endIndex - startIndex);
                    phrases[runs] = search.ToLower();
                    SearchBoxContent = SearchBoxContent.Replace(exact, "");
                    runs++;
                }

                if (SearchBoxContent.Contains(' '))
                {
                    words = SearchBoxContent.Split(' ');
                }

                var content = phrases.Concat(words);
                content = content.Where(u => u != null && u != string.Empty && !u.StartsWith(' ')).ToArray();

                if (content.Any())
                {
                    var comments = await _unitOfWork.Comment.GetAllAsync();
                    comments = comments.ToList().Where(c => content.Any(u => c.Note.ToLower().Contains(u)));
                    var filteredPostList = objPostList.ToList().Where(u => content.Any(c => u.PostDesc.ToLower().Contains(c)));
                    var commentsList = objPostList.Where(u => comments.Any(c => c.PostId == u.Id));
                    objPostList = objPostList.Where(u => filteredPostList.Any(f => f.Id == u.Id) || commentsList.Any(c => c.Id == u.Id));
                }
                else
                {
                    if (SearchBoxContent.IsNullOrEmpty())
                    {
                        objPostList = await _unitOfWork.Post.GetAllAsync(u => u.Enrollment.ClassId == _userSessionService.GetUserSession().ClassId, includes: "Folder");
                    }
                    var singleContent = SearchBoxContent.ToLower();
                    var comments = await _unitOfWork.Comment.GetAllAsync();
                    comments = comments.ToList().Where(c => c.Note.ToLower().Contains(singleContent));
                    IEnumerable<Post> commentsList = objPostList.Where(u => comments.Any(c => c.PostId == u.Id));
                    IEnumerable<Post> filteredPostList = objPostList.ToList().Where(u => u.PostDesc.ToLower().Contains(singleContent));
                    objPostList = objPostList.Where(u => filteredPostList.Any(f => f.Id == u.Id) || commentsList.Any(c => c.Id == u.Id));
                }
            }

            // Filter by folder(s)
            var folderArray = Request.Form["folderSelect"];
            if (!folderArray.Contains("null"))
            {
                objPostList = objPostList.Where(u => folderArray.Any(s => u.FolderId == int.Parse(s)));
            }

            // Filter by date
            var datetime = Request.Form["dateSelect"];
            if (datetime != "null")
            {
                TimeSpan dateTime = TimeSpan.FromDays(Double.Parse(datetime));
                DateTime start = DateTime.Now.Subtract(dateTime);
                DateTime end = DateTime.Now;
                objPostList = objPostList.Where(u => u.Created >= start && u.Created <= end);
            }

            // Filter by status
            var status = Request.Form["statusSelect"];
            if (status != "null")
            {
                objPostList = objPostList.Where(u => u.StatusId == int.Parse(status));
            }

            objFolderList = await _unitOfWork.Folder.GetAllAsync(u => u.ClassId == _userSessionService.GetUserSession().ClassId && u.Approved == true);
            AllFolders = await _unitOfWork.Folder.GetAllAsync(f => f.ClassId == _userSessionService.GetUserSession().ClassId);
            objStatusList = await _unitOfWork.Status.GetAllAsync();
            SearchBoxContent = "";
            return Page();
        }


    }


}
