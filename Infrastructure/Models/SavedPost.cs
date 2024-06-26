using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class SavedPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "User")]
        public string ApplicationUserId { get; set; } // Change the property name and type

        [Required]
        [Display(Name = "Post")]
        public int PostId { get; set; } // Assuming this is the ID of the post being saved

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } // Change the navigation property name

        [ForeignKey("PostId")]
        public Post Post { get; set; } // Assuming you have a Post model
    }
}
