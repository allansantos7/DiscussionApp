using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class PostAttachment
    {
        [Key]
        public int Id { get; set; }
        public string? Url { get; set; }

        [Required]
        [Display(Name = "Post")]
        public int PostId { get; set; }

        [Required]
        [Display(Name = "AttachmentType")]
        public int AttachmentTypeId { get; set; }

        [ForeignKey("PostId")]
        public Post? Post { get; set; }

        [ForeignKey("AttachmentTypeId")]
        public AttachmentType? AttachmentType { get; set; }

    }
}
