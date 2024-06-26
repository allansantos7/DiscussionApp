using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class CommentAttachment
    {
        [Key]
        public int Id { get; set; }

        public string? Url { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public int CommentId { get; set; }

        [Required]
        [Display(Name = "AttachmentType")]
        public int AttachmentTypeId { get; set; }

        [ForeignKey("CommentId")]
        public Comment? Comment { get; set; }

        [ForeignKey("AttachmentId")]
        public AttachmentType? AttachmentType { get; set; }
    }
}
