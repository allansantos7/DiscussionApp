using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Archived { get; set; }
        public SemesterSeason? Semester { get; set; }
        public int? SemesterYear { get; set; }
        
        [NotMapped]
        [Display(Name = "Semester Term")]
        public string SemesterTerm
        {
            get
            {
                return Semester + " " + SemesterYear;
            }
        }
    }

    public enum SemesterSeason
    {
        Spring,
        Summer,
        Fall
    }
}