using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Social_Media_Exchanger.Models
{
    public class TwitterPage : SocialPage
    {

        [Index(IsUnique = true)]
        [Required]
        [MaxLength(50)]
        public string TwitterUsername { get; set; }

        [Required]
        [MaxLength(60)]
        [RegularExpression(@"^http(s)?:\/\/(www\.)?twitter\.com\/\S+$", ErrorMessage = "Your link must be like this")]
        public override string Url { get; set; }
    }
}