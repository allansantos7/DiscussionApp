using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Note
        {
            get; set;
        }

        public int? PostId
        {
            get; set;
        }

        [ForeignKey("PostId")]
        public Post? Post { get; set; }

        public int? EnrollmentId
        {
            get; set;
        }

        [ForeignKey("EnrollmentId")]
        public Enrollment? Enrollment { get; set; }

        public int? CommentId
        {
            get; set;
        }

        [ForeignKey("CommentId")]
        public Comment? Parent { get; set; }
    }
}
