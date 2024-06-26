
using Microsoft.AspNetCore.Authorization;
using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Microsoft.Extensions.Hosting;
using ClosedXML.Excel;

namespace LogicLeapPiazzaRemake.Pages.Admin
{
    [Authorize(Roles = "Instructor")]
    public class ReportsModel : PageModel
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
        [BindProperty]
        public IEnumerable<Enrollment> objInstructorEnrollmentList { get; set; }
        public IEnumerable<Enrollment> objStudentEnrollmentList;
        public IEnumerable<Post> objPostList;
        public IEnumerable<Comment> objCommentList;
        public IEnumerable<CommentAward> objCommentAwardList;
        public IEnumerable<UserTag> objUserTagList;
        public IEnumerable<PostAward> objPostAwardList;
        [BindProperty]
        public Dictionary<string, StudentReportData> reportData { get; set; }

        public ReportsModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            objStudentList = new List<ApplicationUser>();
            objInstructorClassList = new List<Class>();
            objTopicList = new List<Folder>();
            objInstructorEnrollmentList = new List<Enrollment>();
            objPostList = new List<Post>();
            objCommentList = new List<Comment>();
            objCommentAwardList = new List<CommentAward>();
            objUserTagList = new List<UserTag>();
            objPostAwardList = new List<PostAward>();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            reportData = new Dictionary<string, StudentReportData>();

            // List of Enrollments, classes for the classes the Instructor is currently teaching
            objInstructorEnrollmentList = await _unitOfWork.Enrollment.GetAllAsync(u => u.ApplicationUserId == user.Id && u.Status == EnrollmentStatus.Active, includes: "Class");

            // need to limit what is shown to only students for the currently logged in instructor's class
            objStudentEnrollmentList = await _unitOfWork.Enrollment.GetAllAsync(u => u.ApplicationUserId != user.Id && u.Status == EnrollmentStatus.Active, includes: "Class,ApplicationUser");

            // filter enrollments by the Instructor's classes
            var filteredStudentEnrollmentList = objStudentEnrollmentList.Where(u => objInstructorEnrollmentList.Any(y => y.ClassId == u.ClassId));

            // List of Topics/Folders for Instructor's classes
            objTopicList = _unitOfWork.Folder.GetAll(includes: "Class");
            var filteredTopicList = objTopicList.Where(u => objInstructorEnrollmentList.Any(y => y.ClassId == u.ClassId));

            // List of Posts for Instructor's classes
            objPostList = await _unitOfWork.Post.GetAllAsync(includes: "Enrollment,Status,PostType,Folder");
            var filteredPostList = objPostList.Where(u => objStudentEnrollmentList.Any(y => y.Id == u.EnrollmentId));

            // List of Comments for Instructor's classes
            objCommentList = await _unitOfWork.Comment.GetAllAsync(includes: "Post,Enrollment,Parent");
            var filteredCommentList = objCommentList.Where(u => objPostList.Any(y => y.Id == u.PostId));

            // List of tags for all Instructor's students
            objUserTagList = await _unitOfWork.UserTag.GetAllAsync(includes: "Enrollment,Comment");
            var filteredUserTagList = objUserTagList.Where(u => objStudentEnrollmentList.Any(y => y.Id == u.EnrollmentId));

            // List of awards for all Instructor's students
            objCommentAwardList = await _unitOfWork.CommentAward.GetAllAsync(includes: "Comment,Award");
            var filteredCommentAwardList = objCommentAwardList.ToList().Where(u => objCommentList.Any(y => y.Id == u.CommentId));
            objPostAwardList = await _unitOfWork.PostAward.GetAllAsync(includes: "Post,Award");
            var filteredPostAwardList = objPostAwardList.ToList().Where(u => objPostList.Any(y => y.Id == u.PostId));

            // Create List of Student data
            foreach (var objStudent in filteredStudentEnrollmentList)
            {
                // class
                var studentClass = objStudent.Class;
                // posts
                var studentPost = objPostList.ToList().Where(u => u.EnrollmentId == objStudent.Id && u.Enrollment.Class.Id == studentClass.Id);
                // comments
                var studentComment = objCommentList.ToList().Where(u => u.EnrollmentId == objStudent.Id && u.Enrollment.Class.Id == studentClass.Id);
                // endorsements
                var studentCommentEndorsements = objCommentAwardList.ToList().Where(u => studentComment.Any(y => y.Id == u.CommentId && u.Award.Id == 1));
                var studentPostEndorsements = objPostAwardList.ToList().Where(u => studentPost.Any(y => y.Id == u.PostId && u.Award.Id == 1));
                // tags
                var studentTag = objUserTagList.ToList().Where(u => u.EnrollmentId == objStudent.Id && u.Enrollment.Class.Id == studentClass.Id);
                // upvotes
                var studentCommentUpvotes = objCommentAwardList.ToList().Where(u => studentComment.Any(y => y.Id == u.CommentId && u.Award.Id == 2));
                var studentPostUpvotes = objPostAwardList.ToList().Where(u => studentPost.Any(y => y.Id == u.PostId && u.Award.Id == 2));

                // If the student name already exists in the dictionary, aggregate the data
                string studentName = objStudent.ApplicationUser.FullName;
                if (reportData.ContainsKey(studentName + " - " + studentClass.Id))
                {
                    reportData[studentName].Posts = studentPost.Count();
                    reportData[studentName].Comments = studentComment.Count();
                    reportData[studentName].Endorsements += (studentCommentEndorsements.Count() + studentPostEndorsements.Count());
                    reportData[studentName].Tags += studentTag.Count();
                    reportData[studentName].Upvotes += (studentCommentUpvotes.Count() + studentPostUpvotes.Count());
                }
                // If the student name doesn't exist in the dictionary, create a new StudentReportData object
                else
                {
                    reportData[studentName + " - " + studentClass.Id] = new StudentReportData
                    {
                        ClassName = studentClass.SemesterTerm + " - " + studentClass.Name,
                        Name = studentName,
                        Posts = studentPost.Count(),
                        Comments = studentComment.Count(),
                        Endorsements = studentCommentEndorsements.Count() + studentPostEndorsements.Count(),
                        Tags = studentTag.Count(),
                        Upvotes = studentCommentUpvotes.Count() + studentPostUpvotes.Count(),
                    };
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string reportData, string action)
        {
            // Translate data into Dictionary object
            var rDataJson = JsonConvert.DeserializeObject<Dictionary<string, StudentReportData>>(reportData);

            if (action == "DownloadPDF")
            {
                #region PDFCreation
                // Generate Pdf
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Student Report";

                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 12);  // Using a slightly larger font for clarity

                double yPos = 40; // Initial vertical position for the first line
                double xPos = 40; // Horizontal margin
                double lineSpacing = 20; // Space between lines

                foreach (var reportItem in rDataJson)
                {

                    // Format content
                    List<string> contentLines = new List<string> {
                        "Class: " + reportItem.Value.ClassName,
                        "Name: " + reportItem.Value.Name,
                        "Posts: " + reportItem.Value.Posts.ToString(),
                        "Comments: " + reportItem.Value.Comments.ToString(),
                        "Endorsements: " + reportItem.Value.Endorsements.ToString(),
                        "Tags: " + reportItem.Value.Tags.ToString(),
                        "Upvotes: " + reportItem.Value.Upvotes.ToString()
                    };

                    foreach (string line in contentLines)
                    {
                        // Check if new page is needed before adding each line
                        if (yPos + lineSpacing > page.Height - 40) // Adjust bottom margin as needed
                        {
                            page = document.AddPage(); // Add a new page
                            gfx = XGraphics.FromPdfPage(page); // Get graphics for new page
                            yPos = 40; // Reset y position for the new page
                        }

                        // Draw the string onto the page
                        gfx.DrawString(line, font, XBrushes.Black, new XRect(xPos, yPos, page.Width - 2 * xPos, page.Height), XStringFormats.TopLeft);
                        yPos += lineSpacing; // Move yPos for the next line
                    }
                    yPos += lineSpacing; // Optionally add extra spacing between entries
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream, false);
                    return File(stream.ToArray(), "application/pdf", "All_Students_Report.pdf");
                }
                #endregion
            }
            else
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Student Report");
                    int row = 1;

                    // Define headers
                    worksheet.Cell(row, 1).Value = "Class";
                    worksheet.Cell(row, 2).Value = "Name";
                    worksheet.Cell(row, 3).Value = "Posts";
                    worksheet.Cell(row, 4).Value = "Comments";
                    worksheet.Cell(row, 5).Value = "Endorsements";
                    worksheet.Cell(row, 6).Value = "Tags";
                    worksheet.Cell(row, 7).Value = "Upvotes";
                    row++;

                    // Fill worksheet with data
                    foreach (var reportItem in rDataJson)
                    {
                        worksheet.Cell(row, 1).Value = reportItem.Value.ClassName;
                        worksheet.Cell(row, 2).Value = reportItem.Value.Name;
                        worksheet.Cell(row, 3).Value = reportItem.Value.Posts;
                        worksheet.Cell(row, 4).Value = reportItem.Value.Comments;
                        worksheet.Cell(row, 5).Value = reportItem.Value.Endorsements;
                        worksheet.Cell(row, 6).Value = reportItem.Value.Tags;
                        worksheet.Cell(row, 7).Value = reportItem.Value.Upvotes;
                        row++;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Student_Report.xlsx");
                    }
                }
            }
        }

        public class StudentReportData
        {
            // Class of Student's activity
            public string ClassName { get; set; }
            // name of student
            public string Name { get; set; }
            // Number of posts
            public int Posts {  get; set; }
            // Number of replies
            public int Comments { get; set; }
            // Number of endorsements
            public int Endorsements { get; set; }
            // User tags
            public int Tags { get; set; }
            // Number of Upvotes
            public int Upvotes { get; set; }
        }
    }
}
