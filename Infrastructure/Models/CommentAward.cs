using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class CommentAward
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CommentId { get; set; }

        [ForeignKey("CommentId")]
        public Comment? Comment { get; set; }

        [Required]
        public int AwardId { get; set; }

        [ForeignKey("AwardId")]
        public Award? Award { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
        [Required]
        public string? ApplicationUserId { get; set; }
    }
}
