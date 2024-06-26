using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? PostTitle { get; set; }

        [Required]
        public string? PostDesc { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        [Required]
        public int EnrollmentId { get; set; }

        [ForeignKey("EnrollmentId")]
        public Enrollment? Enrollment { get; set; }

        [Required]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public Status? Status { get; set; }

        [Required]
        public int PostTypeId { get; set; }

        [ForeignKey("PostTypeId")]
        public  PostType? PostType { get; set; }

        [Required]
        public int FolderId { get; set; }

        [ForeignKey("FolderId")]
        public Folder? Folder { get; set; }
    }
}
