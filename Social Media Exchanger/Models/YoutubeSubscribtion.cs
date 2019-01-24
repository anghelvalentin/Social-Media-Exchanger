using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Social_Media_Exchanger.Models
{
    public class YoutubeSubscribtion
    {
        [Key, Column(Order = 1)]
        public int YoutubePageId { get; set; }

        public YoutubePage YoutubePage { get; set; }


        [Key, Column(Order = 2)]
        public String UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}