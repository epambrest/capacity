﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Models
{
    public class AspNetRoleClaims
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty(nameof(AspNetRoles.AspNetRoleClaims))]
        public virtual AspNetRoles Role { get; set; }
    }
}
