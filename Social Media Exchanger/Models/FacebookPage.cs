using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Social_Media_Exchanger.Models
{
    public class FacebookPage : SocialPage
    {
        [Index(IsUnique = true)]
        public long FacebookCode { get; set; }

        [RegularExpression(@"^http(s)?://(w{3}\.)?facebook\S+$", ErrorMessage = "Your link must be like this http://facebook.com/name")]
        public override string Url { get; set; }
    }
}