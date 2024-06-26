﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Award
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AwardName { get; set; }
    }
}
