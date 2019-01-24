using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Social_Media_Exchanger.Models;

namespace Social_Media_Exchanger.ViewModels
{
    public class MySocialPagesViewModel
    {
        public IEnumerable<FacebookPage> FacebookPages { get; set; }

        public IEnumerable<TwitterPage> TwitterPages { get; set; }

        public IEnumerable<YoutubePage> YoutubePages { get; set; }

        public int numberOfPages { get; set; }
    }
}