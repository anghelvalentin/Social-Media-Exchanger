using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Social_Media_Exchanger.Models
{
    public class YoutubePage : SocialPage
    {
        [Required]
        [MaxLength(50)]
        public string YoutubeUsername { get; set; }

        [Required]
        [MaxLength(80)]
        public override string Url { get; set; }
    }
}