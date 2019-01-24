using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Social_Media_Exchanger.Models
{
    public class Refferal
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        //[Index(IsUnique = true)]
        //[Required]
        //[MaxLength(300)]
        //public string ReffId { get; set; }

        public ApplicationUser RefferalUser { get; set; }

        [Required]
        public bool Activate { get; set; }

        public Refferal()
        {
            Activate = false;
        }
    }
}