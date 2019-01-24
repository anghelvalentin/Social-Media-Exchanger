using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Social_Media_Exchanger.Models;
using System.ComponentModel.DataAnnotations;

namespace Social_Media_Exchanger.ViewModels
{
    public class SocialPageFormViewModel
    {
        [Display(Name = "Social Network")]
        [Required]
        public int SocialNetworkId { get; set; }
        public IEnumerable<SocialNetwork> SocialNetworks { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(350)]
        [Url(ErrorMessage = "You must put a link here")]
        [LinkValid]
        public string Url { get; set; }

        [Range(2, 10)]
        public int Cpc { get; set; }
    }
}