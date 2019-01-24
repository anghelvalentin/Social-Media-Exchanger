using Social_Media_Exchanger.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Social_Media_Exchanger.Models
{
    public class LinkValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var socialPage = (SocialPageFormViewModel)validationContext.ObjectInstance;

            if (String.IsNullOrWhiteSpace(socialPage.Url))
            {
                return new ValidationResult("You must set a link");
            }
            else if (socialPage.SocialNetworkId == SocialNetwork.FacebookPageLikes)
            {
                if (Regex.IsMatch(socialPage.Url, @"^http(s)?://(w{3}\.)?facebook\S+$"))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Your link must be like this https://facebook.com/name");

            }
            else if (socialPage.SocialNetworkId == SocialNetwork.TwitterFollowers)
            {
                if (Regex.IsMatch(socialPage.Url, @"^http(s)?:\/\/(www\.)?twitter\.com\/\S+$"))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Your link must be like this https://twitter.com/username");
            }
            else if (socialPage.SocialNetworkId == SocialNetwork.YoutubeSubscribers)
            {
                if (Regex.IsMatch(socialPage.Url, @"^http(s)?:\/\/(www\.)?youtube\.com\/(channel|user)\S+$"))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Your link must be like this https://www.youtube.com/user/username");
            }
            else
            {
                return new ValidationResult("Please enter valid values");
            }
        }
    }
}