﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportService.Models
{
    public class nodeItems
    {

        [Key]

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }

        public string? address { get; set; }
    }
}
