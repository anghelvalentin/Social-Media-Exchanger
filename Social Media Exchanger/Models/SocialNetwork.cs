using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Social_Media_Exchanger.Models
{
    public class SocialNetwork
    {
        public int SocialNetworkId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }


        public static readonly int FacebookPageLikes = 1;
        public static readonly int TwitterFollowers = 2;
        public static readonly int YoutubeSubscribers = 3;
    }
}