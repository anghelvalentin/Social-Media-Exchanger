using Social_Media_Exchanger.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Social_Media_Exchanger.ViewModels
{
    public class EditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Range(1, 3), Required]
        public int SocialNetworkId { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public bool Active { get; set; }

        [Range(2, 10), Required]
        public int Cpc { get; set; }
    }
}