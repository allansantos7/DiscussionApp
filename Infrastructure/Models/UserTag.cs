using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class UserTag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name ="Enrollment")]
        public int EnrollmentId { get; set; }

        [Required]
        [Display(Name ="Comment")]
        public int CommentId { get; set; }

        [ForeignKey("EnrollmentId")]
        public Enrollment? Enrollment { get; set; }

        [ForeignKey("CommentId")]
        public Comment? Comment { get; set; }
    }
}
