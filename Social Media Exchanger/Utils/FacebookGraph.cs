using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.WebPages.Html;

namespace Social_Media_Exchanger.Utils
{
    public class FacebookGraph
    {
        public string Name { get; set; }

        public FacebookGraph(string url)
        {
            Name = takeName(url);
        }

        private string takeName(string Url)
        {
            string text = Regex.Match(Url, @"(?<=\.com\/)[^\s\/]+").ToString();

            if (String.IsNullOrWhiteSpace(text))
            {
                throw new BadLinkException("Please enter a valid facebook page");
            }


            var matchValues = Regex.Match(text, @"\d{7,}");
            if (matchValues.Success)
            {
                return matchValues.ToString();
            }
            else
            {
                return text;
            }
        }


        public long takeFacebookId()
        {
            JObject obiectFb = getDataFacebookObject();
            if (obiectFb != null)
            {
                return long.Parse(obiectFb.GetValue("id").ToString());
            }
            else
            {
                throw new BadLinkException("This page doesn't exist");
            }
        }


        public JObject getDataFacebookObject()
        {
            JObject graph = null;
            using (WebClient web = new WebClient())
            {
                string source = null;
                try
                {
                    source = web.DownloadString("https://graph.facebook.com/" + Name + "/?fields=fan_count&access_token=236473693414165|0a938d5a73ec187cc10a0ede3be1c077");
                    graph = JObject.Parse(source);
                }
                catch (WebException e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            return graph;
        }

        public string getNumberOfLikes()
        {
            JObject graph = getDataFacebookObject();
            if (graph != null)
            {
                var numberOfLikes = graph.GetValue("fan_count").ToString();
                if (!String.IsNullOrWhiteSpace(numberOfLikes))
                {
                    return numberOfLikes;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}