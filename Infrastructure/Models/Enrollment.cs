using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        public int? ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; }

        public string? ApplicationUserId
        {
            get; set;
        }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser
        {
            get; set;
        }

        //public int? RoleId
        //{
        //    get; set;
        //}

        //[ForeignKey("RoleId")]
        //public string? Role
        //{
        //    get; set;
        //}

        [Required]
        public DateTime Created {  get; set; }

        public EnrollmentStatus? Status { get; set; }
    }

    public enum EnrollmentStatus
    {
        Inactive,
        Active
    }
}