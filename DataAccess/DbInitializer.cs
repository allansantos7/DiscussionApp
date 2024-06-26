using DataAccess;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Utility;


namespace DataAccess
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public void Initialize()
        {
            _db.Database.EnsureCreated();

            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            if (_db.Classes.Any())
            {
                return; //DB has been seeded
            }

            // create roles if they are not created
            _roleManager.CreateAsync(new IdentityRole(SD.InstructorRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.StudentRole)).GetAwaiter().GetResult();

            // create super user
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "logic@leap.com",
                Email = "logic@leap.com",
                FirstName = "Logic",
                LastName = "Leap",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Admin123*").GetAwaiter().GetResult();

            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "logic@leap.com");
            _userManager.AddToRoleAsync(user, SD.InstructorRole).GetAwaiter().GetResult();

            // create Instructor user 1
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "instructor1@weber.edu",
                Email = "instructor1@weber.edu",
                FirstName = "Inst",
                LastName = "Ructor1",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Instructor123*").GetAwaiter().GetResult();

            ApplicationUser instructorUser1 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "instructor1@weber.edu");
            _userManager.AddToRoleAsync(instructorUser1, SD.InstructorRole).GetAwaiter().GetResult();

            // create Instructor user 2
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "instructor2@weber.edu",
                Email = "instructor2@weber.edu",
                FirstName = "Inst",
                LastName = "Ructor2",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Instructor123*").GetAwaiter().GetResult();

            ApplicationUser instructorUser2 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "instructor2@weber.edu");
            _userManager.AddToRoleAsync(instructorUser2, SD.InstructorRole).GetAwaiter().GetResult();

            // create Instructor user 3
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "instructor3@weber.edu",
                Email = "instructor3@weber.edu",
                FirstName = "Inst",
                LastName = "Ructor3",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Instructor123*").GetAwaiter().GetResult();

            ApplicationUser instructorUser3 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "instructor3@weber.edu");
            _userManager.AddToRoleAsync(instructorUser3, SD.InstructorRole).GetAwaiter().GetResult();

            // Create Student User 1
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "student1@mail.weber.edu",
                Email = "student1@mail.weber.edu",
                FirstName = "Stu",
                LastName = "Dent",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Student123*").GetAwaiter().GetResult();

            ApplicationUser user1 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "student1@mail.weber.edu");
            _userManager.AddToRoleAsync(user1, SD.StudentRole).GetAwaiter().GetResult();

            // Create Student User 2
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "student2@mail.weber.edu",
                Email = "student2@mail.weber.edu",
                FirstName = "Stu",
                LastName = "Dent2",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Student123*").GetAwaiter().GetResult();

            ApplicationUser user2 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "student2@mail.weber.edu");
            _userManager.AddToRoleAsync(user2, SD.StudentRole).GetAwaiter().GetResult();

            // Create Student User 3
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "student3@mail.weber.edu",
                Email = "student3@mail.weber.edu",
                FirstName = "Stu",
                LastName = "Dent3",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Student123*").GetAwaiter().GetResult();

            ApplicationUser user3 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "student3@mail.weber.edu");
            _userManager.AddToRoleAsync(user3, SD.StudentRole).GetAwaiter().GetResult();

            // Create Student User 4
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "student4@mail.weber.edu",
                Email = "student4@mail.weber.edu",
                FirstName = "Stu",
                LastName = "Dent4",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Student123*").GetAwaiter().GetResult();

            ApplicationUser user4 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "student4@mail.weber.edu");
            _userManager.AddToRoleAsync(user4, SD.StudentRole).GetAwaiter().GetResult();

            // Create Student User 5
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "student5@mail.weber.edu",
                Email = "student5@mail.weber.edu",
                FirstName = "Stu",
                LastName = "Dent5",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Student123*").GetAwaiter().GetResult();

            ApplicationUser user5 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "student5@mail.weber.edu");
            _userManager.AddToRoleAsync(user5, SD.StudentRole).GetAwaiter().GetResult();

            // Create Student User 6
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "student6@mail.weber.edu",
                Email = "student6@mail.weber.edu",
                FirstName = "Stu",
                LastName = "Dent6",
                StreetAddress = "123 Logic Leap Blvd",
                City = "Layton",
                State = "UT",
                PostalCode = "84056",
            }, "Student123*").GetAwaiter().GetResult();

            ApplicationUser user6 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "student6@mail.weber.edu");
            _userManager.AddToRoleAsync(user6, SD.StudentRole).GetAwaiter().GetResult();

            // Create seed data for Classes
            var Classes = new List<Class>
            {
                // Class 1
                new Class
                {
                    Name = "Software Engineering II",
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    Semester = SemesterSeason.Spring,
                    SemesterYear = 2024,
                },
                // Class 2
                new Class
                {
                    Name = "Advanced Database Programming",
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    Semester = SemesterSeason.Spring,
                    SemesterYear = 2024,
                },
                // Class 3
                new Class
                {
                    Name = "Formal Language & Algorithms",
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    Semester = SemesterSeason.Spring,
                    SemesterYear = 2024,
                }
            };
            foreach (var c in Classes)
            {
                _db.Classes.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for Enrollment
            var Enrollments = new List<Enrollment>
            {
                // Class 1 Students and Instructor
                // Enrollment 1 for Student 1
                new Enrollment
                {
                    ClassId = 1,
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    ApplicationUserId = user1.Id,
                    Status = EnrollmentStatus.Active,
                },
                // Enrollment 2 for Student 2
                new Enrollment
                {
                    ClassId = 1,
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    ApplicationUserId = user2.Id,
                    Status = EnrollmentStatus.Active,
                },
                // Enrollment 3 for Instructor 1
                new Enrollment
                {
                    ClassId = 1,
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    ApplicationUserId = instructorUser1.Id,
                    Status = EnrollmentStatus.Active,
                },

                // Class 2 Students and Instructor
                // Enrollment 4 for Student 3
                new Enrollment
                {
                    ClassId = 2,
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    ApplicationUserId = user3.Id,
                    Status = EnrollmentStatus.Active,
                },
                // Enrollment 5 for Student 4
                new Enrollment
                {
                    ClassId = 2,
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    ApplicationUserId = user4.Id,
                    Status = EnrollmentStatus.Active,
                },
                // Enrollment 6 for Instructor 2
                new Enrollment
                {
                    ClassId = 2,
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    ApplicationUserId = instructorUser2.Id,
                    Status = EnrollmentStatus.Active,
                },

                // Class 3 Students and Instructor
                // Enrollment 7 for Student 5
                new Enrollment
                {
                    ClassId = 3,
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    ApplicationUserId = user5.Id,
                    Status = EnrollmentStatus.Active,
                },
                // Enrollment 8 for Student 6
                new Enrollment
                {
                    ClassId = 3,
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    ApplicationUserId = user6.Id,
                    Status = EnrollmentStatus.Active,
                },
                // Enrollment 9 for Instructor 3
                new Enrollment
                {
                    ClassId = 3,
                    Created = new DateTime(2024, 01, 01, 01, 0, 0),
                    ApplicationUserId = instructorUser3.Id,
                    Status = EnrollmentStatus.Active,
                },
            };

            foreach (var c in Enrollments)
            {
                _db.Enrollments.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for Folder
            var Folders = new List<Folder>
            {
                // Folder 1 for Class 1
                new Folder
                {
                    Name = "CS 3750",
                    ClassId = 1,
                },
                // Subfolders for Class 1
                // Folder 2
                // Assignment 1
                new Folder
                {
                    Name = "Assignment 1",
                    ClassId = 1,
                    FolderId = 1,
                },
                // Folder 3
                // Quiz 1
                new Folder
                {
                    Name = "Quiz 1",
                    ClassId = 1,
                    FolderId = 1,
                },

                // Folder 4 for Class 2
                new Folder
                {
                    Name = "CS 3550",
                    ClassId = 2,
                },
                // Subfolders for Class 2
                // Folder 5
                // Assignment 1
                new Folder
                {
                    Name = "Assignment 1",
                    ClassId = 2,
                    FolderId = 4,
                },
                // Folder 6
                // Quiz 1
                new Folder
                {
                    Name = "Quiz 1",
                    ClassId = 2,
                    FolderId = 4,
                },

                // Folder 7 for Class 3
                new Folder
                {
                    Name = "CS 4110",
                    ClassId = 3,
                },
                // Subfolders for Class 3
                // Folder 8
                // Assignment 1
                new Folder
                {
                    Name = "Assignment 1",
                    ClassId = 3,
                    FolderId = 7,
                },
                // Folder 9
                // Quiz 1
                new Folder
                {
                    Name = "Quiz 1",
                    ClassId = 3,
                    FolderId = 7,
                },
            };

            foreach (var c in Folders)
            {
                _db.Folders.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for Status
            var Statuses = new List<Status>
            {
                // Status 1
                new Status
                {
                    StatusName = "Open",
                },
                // Status 2
                new Status
                {
                    StatusName = "Closed",
                },
                // Status 3
                new Status
                {
                    StatusName = "Answered by Student",
                },
                // Status 4
                new Status
                {
                    StatusName = "Answered by Instructor",
                },
            };

            foreach (var c in Statuses)
            {
                _db.Statuses.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for PostType
            var PostTypes = new List<PostType>
            {
                // PostType 1
                new PostType
                {
                    TypeName = "Question",
                },
                // PostType 2
                new PostType
                {
                    TypeName = "Issue",
                },
                // PostType 3
                new PostType
                {
                    TypeName = "PSA",
                },
            };

            foreach (var c in PostTypes)
            {
                _db.PostTypes.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for Posts
            var Posts = new List<Post>
            {
                // Class 1
                // Post 1, Class Question
                // Posted By Student 1
                // Commented by Student 2, Answered
                new Post
                {
                    PostTitle = "Question Post TEST w/ a Reply and Attachments",
                    PostDesc = "Question Post TEST, A reply, has image attachment, Question Answered by Student",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    EnrollmentId = 1,
                    StatusId = 3,
                    PostTypeId = 1,
                    FolderId = 1,
                },
                // Post 2, Assignment Issue
                // Posted By Student 2
                // Commented by Student 1, Unanswered
                new Post
                {
                    PostTitle = "Assignment Issue Post TEST w/ a Reply",
                    PostDesc = "Assignment Issue Post TEST, A reply, No Attachments, Issue Not Answered by Student",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    EnrollmentId = 2,
                    StatusId = 1,
                    PostTypeId = 2,
                    FolderId = 2,
                },
                // Post 3, Quiz PSA
                // Posted by Student 2
                // Closed to Comments
                new Post
                {
                    PostTitle = "Quiz PSA Post TEST",
                    PostDesc = "Quiz PSA Post TEST, Closed to Replies",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    EnrollmentId = 2,
                    StatusId = 2,
                    PostTypeId = 3,
                    FolderId = 3,
                },

                // Class 2
                // Post 4, Class Question
                // Posted By Student 3
                // Commented by Student 4, Answered
                new Post
                {
                    PostTitle = "Question Post TEST w/ a Reply and Attachments",
                    PostDesc = "Question Post TEST, A reply, has image attachment, Question Answered by Student",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    EnrollmentId = 4,
                    StatusId = 3,
                    PostTypeId = 1,
                    FolderId = 4,
                },
                // Post 5, Assignment Issue
                // Posted By Student 4
                // Commented by Student 3, Unanswered
                new Post
                {
                    PostTitle = "Assignment Issue Post TEST w/ a Reply",
                    PostDesc = "Assignment Issue Post TEST, A reply, No Attachments, Issue Not Answered by Student",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    EnrollmentId = 5,
                    StatusId = 1,
                    PostTypeId = 2,
                    FolderId = 5,
                },
                // Post 6, Quiz PSA
                // Posted by Student 3
                // Closed to Comments
                new Post
                {
                    PostTitle = "Quiz PSA Post TEST",
                    PostDesc = "Quiz PSA Post TEST, Closed to Replies",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    EnrollmentId = 4,
                    StatusId = 2,
                    PostTypeId = 3,
                    FolderId = 6,
                },

                // Class 3
                // Post 7, Class Question
                // Posted By Student 5
                // Commented by Student 6, Answered
                new Post
                {
                    PostTitle = "Question Post TEST w/ a Reply and Attachments",
                    PostDesc = "Question Post TEST, A reply, has image attachment, Question Answered by Student",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    EnrollmentId = 7,
                    StatusId = 3,
                    PostTypeId = 1,
                    FolderId = 7,
                },
                // Post 8, Assignment Issue
                // Posted By Student 6
                // Commented by Student 5, Unanswered
                new Post
                {
                    PostTitle = "Assignment Issue Post TEST w/ a Reply",
                    PostDesc = "Assignment Issue Post TEST, A reply, No Attachments, Issue Not Answered by Student",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    EnrollmentId = 8,
                    StatusId = 1,
                    PostTypeId = 2,
                    FolderId = 8,
                },
                // Post 9, Quiz PSA
                // Posted by Student 6
                // Closed to Comments
                new Post
                {
                    PostTitle = "Quiz PSA Post TEST",
                    PostDesc = "Quiz PSA Post TEST, Closed to Replies",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    EnrollmentId = 8,
                    StatusId = 2,
                    PostTypeId = 3,
                    FolderId = 9,
                },
            };

            foreach (var c in Posts)
            {
                _db.Posts.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for Attachment types
            var AttachmentTypes = new List<AttachmentType>
            {
                // Attachment Type 1
                new AttachmentType
                {
                    TypeName = "image",
                },
                // Attachment Type 2
                new AttachmentType
                {
                    TypeName = "file",
                },
                // Attachment Type 3
                new AttachmentType
                {
                    TypeName = "video",
                }
            };

            foreach (var c in AttachmentTypes)
            {
                _db.AttachmentTypes.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for Post Attachment
            var PostAttachments = new List<PostAttachment>
            {
                // Post Attachment 1 to Post 1
                // Image
                new PostAttachment
                {
                    Url = "/attachments/images/Cheetos1.jpg",
                    PostId = 1,
                    AttachmentTypeId = 1,
                },
                // Post Attachment 2 to Post 2
                // Image
                new PostAttachment
                {
                    Url = "/attachments/images/Cheetos2.jpg",
                    PostId = 2,
                    AttachmentTypeId = 1,
                },
                // Post Attachment 3 to Post 3
                // Image
                new PostAttachment
                {
                    Url = "/attachments/images/Cheetos3.jpg",
                    PostId = 3,
                    AttachmentTypeId = 1,
                }
            };

            foreach (var c in PostAttachments)
            {
                _db.PostAttachments.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for Comment
            var Comments = new List<Comment>
            {
                // Class 1
                // Comment 1 to Post 1
                // Commented by Student 2
                new Comment
                {
                    Note = "Comment test, has file attachment",
                    PostId = 1,
                    EnrollmentId = 2,
                },
                // Comment 2 to Post 2
                // Commented by Student 1
                new Comment
                {
                    Note = "Comment test, no attachment",
                    PostId = 2,
                    EnrollmentId = 1,
                },

                // Class 2
                // Comment 3 to Post 4
                // Commented by Student 4
                new Comment
                {
                    Note = "Comment test, has file attachment",
                    PostId = 4,
                    EnrollmentId = 5,
                },
                // Comment 4 to Post 5
                // Commented by Student 3
                new Comment
                {
                    Note = "Comment test, no attachment",
                    PostId = 5,
                    EnrollmentId = 4,
                },
                
                // Class 3
                // Comment 5 to Post 7
                // Commented by Student 6
                new Comment
                {
                    Note = "Comment test, has file attachment",
                    PostId = 7,
                    EnrollmentId = 8,
                },
                // Comment 6 to Post 8
                // Commented by Student 5
                new Comment
                {
                    Note = "Comment test, no attachment",
                    PostId = 8,
                    EnrollmentId = 7,
                },
            };

            foreach (var c in Comments)
            {
                _db.Comments.Add(c);
            }
            _db.SaveChanges();

            // create seed data for Comment Attachment
            var CommentAttachments = new List<CommentAttachment>
            {
                // File attachment 1 to Comment 1
                new CommentAttachment
                {
                    Url = "/attachments/files/testfile1.txt",
                    CommentId = 1,
                    AttachmentTypeId = 2,
                },

                // File attachment 2 to Comment 3
                new CommentAttachment
                {
                    Url = "/attachments/files/testfile2.txt",
                    CommentId = 3,
                    AttachmentTypeId = 2,
                },

                // File attachment 3 to Comment 5
                new CommentAttachment
                {
                    Url = "/attachments/files/testfile3.txt",
                    CommentId = 5,
                    AttachmentTypeId = 2,
                },
            };

            foreach (var c in CommentAttachments)
            {
                _db.CommentAttachments.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for Award
            var Awards = new List<Award>
            {
                // Award 1 for when an Instructor endorses a comment
                new Award
                {
                    AwardName = "Endorsed by the Instructor",
                }
            };

            foreach (var c in Awards)
            {
                _db.Awards.Add(c);
            }
            _db.SaveChanges();

            // Create seed data for User tag
            var UserTags = new List<UserTag>
            {
                // Tag for Student 1 to see Student 2's comment 1 to Post 1
                new UserTag
                {
                    EnrollmentId = 1,
                    CommentId = 1,
                },
                // Tag for Student 2 to see Student 1's comment 2 to Post 2
                new UserTag
                {
                    EnrollmentId = 2,
                    CommentId = 2,
                },

                // Tag for Student 3 to see Student 4's comment 3 to Post 4
                new UserTag
                {
                    EnrollmentId = 4,
                    CommentId = 3,
                },
                // Tag for Student 4 to see Student 3's comment 4 to Post 2
                new UserTag
                {
                    EnrollmentId = 5,
                    CommentId = 4,
                },

                // Tag for Student 5 to see Student 6's comment 5 to Post 7
                new UserTag
                {
                    EnrollmentId = 7,
                    CommentId = 5,
                },
                // Tag for Student 6 to see Student 5's comment 6 to Post 8
                new UserTag
                {
                    EnrollmentId = 8,
                    CommentId = 6,
                },
            };

            foreach (var c in UserTags)
            {
                _db.UserTags.Add(c);
            }
            _db.SaveChanges();
        }
    }
}
