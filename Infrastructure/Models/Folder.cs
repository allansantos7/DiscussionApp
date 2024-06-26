using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public Class Class { get; set; }

        public int? FolderId { get; set; }

        [ForeignKey("FolderId")]
        public Folder ParentFolder { get; set; }

        public bool? Approved { get; set; } // Nullable boolean attribute
    }
}
