using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Social_Media_Exchanger.ViewModels;
using Microsoft.AspNet.Identity;
using Social_Media_Exchanger.Models;
using Social_Media_Exchanger.Utils;
using LinqToTwitter;
using System.Text.RegularExpressions;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;

namespace Social_Media_Exchanger.Controllers
{
    [Authorize]
    public class SocialPageController : Controller
    {
        ApplicationDbContext _context;
        TwitterContext _twitterContext;


        public SocialPageController()
        {
            _context = new ApplicationDbContext();
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = "no3NWVxQRKC7Sw5bEFeN1PDtr",
                    ConsumerSecret = "NNoi1t2i05jiFlirXCWC54O91c7KJSeSke6ywSiJa08wU3DKO0",
                    AccessToken = "383165481-4IdMQqBohYjkvUUwwuLlHHrYWZxCeGkVayex1hlP",
                    AccessTokenSecret = "mFcpuXC92juYiEJ89QbC8Rmxi1F6X9ID5U78mk2QHiwDD"
                }
            };

            _twitterContext = new TwitterContext(auth);
        }


        [HttpPost]
        public ActionResult Add(SocialPageFormViewModel model)
        {
            bool error = false;
            var listaSocialNetworks = _context.SocialNetworks.ToList();
            if (!ModelState.IsValid)
            {
                model.SocialNetworks = listaSocialNetworks;
                return View("Add", model);
            }

            try
            {
                switch (model.SocialNetworkId)
                {
                    case 1:
                        addFacebookPage(model);
                        break;

                    case 2:
                        addTwitterFollow(model);
                        break;
                    case 3:
                        addYoutubeSubscriber(model);
                        break;
                    default:
                        throw new Exception("Please enter valid values");
                }
            }
            catch (BadLinkException e)
            {
                ModelState.AddModelError("", e.Message);
                error = true;
            }
            catch (LinkAlreadyExistException e)
            {
                ModelState.AddModelError("", e.Message);
                error = true;
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Please add a valid page");
                error = true;
            }

            if (error)
            {
                model.SocialNetworks = listaSocialNetworks;
                return View("Add", model);
            }


            _context.SaveChanges();
            return RedirectToAction("index", "home");
        }

        [NonAction]
        private void addFacebookPage(SocialPageFormViewModel model)
        {
            var url = Regex.Match(model.Url, @"http(s)?:\/\/(www\.)?facebook\.com\/[^\/\s]+").ToString();

            if (String.IsNullOrWhiteSpace(url))
            {
                throw new BadLinkException("Please enter a valid facebook page");
            }
            FacebookGraph fbGraph = new FacebookGraph(url);

            FacebookPage fbPage = new FacebookPage()
            {
                Name = model.Name,
                Url = url,
                Cpc = model.Cpc
            };


            var facebookId = fbGraph.takeFacebookId();
            var fbAlreadyAdded = _context.Facebooks.SingleOrDefault(fb => fb.FacebookCode == facebookId);
            var currentUserId = User.Identity.GetUserId();

            if (fbAlreadyAdded == null)
            {
                fbPage.FacebookCode = facebookId;
                fbPage.UserId = currentUserId;

                _context.Facebooks.Add(fbPage);
            }
            else
            {
                if (fbAlreadyAdded.UserId == currentUserId)
                {
                    fbAlreadyAdded.Deleted = false;
                }
                else
                {
                    throw new LinkAlreadyExistException("Link is already added on this social network");
                }
            }

        }


        [NonAction]
        private void addTwitterFollow(SocialPageFormViewModel model)
        {
            var username = Regex.Match(model.Url, @"(?<=\.com\/)[^\/\s]+").ToString();
            if (String.IsNullOrWhiteSpace(username))
            {
                throw new BadLinkException("Please enter a valid twitter url");
            }

            var user = _twitterContext.User.Where(u => u.Type == UserType.Show && u.ScreenName == username).SingleOrDefault();
            if (user == null || user.Protected)
            {
                throw new BadLinkException("This user doesn't exist or it's private");
            }

            var currentUserId = User.Identity.GetUserId();

            var twitter = _context.Twitters.SingleOrDefault(twt => twt.TwitterUsername == username);
            if (twitter != null)
            {
                if (twitter.UserId.Equals(currentUserId))
                {
                    twitter.Deleted = false;
                }
                else
                {
                    throw new LinkAlreadyExistException("This twitter accounts is already added on this site");
                }
            }
            else
            {
                TwitterPage twitterPage = new TwitterPage()
                {
                    Url = "http://twitter.com/" + username,
                    TwitterUsername = username,
                    Name = model.Name,
                    Cpc = model.Cpc,
                    UserId = currentUserId
                };

                _context.Twitters.Add(twitterPage);
            }
        }

        [NonAction]
        private void addYoutubeSubscriber(SocialPageFormViewModel model)
        {
            var url = Regex.Match(model.Url, @"http(s):\/\/(www\.)youtube\.com\/(user|channel)\/[^\/\s]+").ToString();
            string usernameOrId = Regex.Match(url, @"(?<=channel\/|user\/)[^\/\s]+").ToString();

            if (String.IsNullOrWhiteSpace(url) || String.IsNullOrWhiteSpace(usernameOrId))
            {
                throw new BadLinkException("This youtube link isn't good. Try to add something like this https://youtube.com/channel/youtube");
            }

            YoutubePage newYoutubePage = new YoutubePage()
            {
                Cpc = model.Cpc,
                Name = model.Name,
                Url = url
            };


            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDp5rhMI0dW7uHIYhq0_iLS9vW5pgy1oy0",
            });

            var request = youtubeService.Channels.List("statistics");

            if (model.Url.Contains("channel"))
            {
                request.Id = usernameOrId;
            }
            else
            {
                request.ForUsername = usernameOrId;
            }

            var response = request.Execute();

            var userRetrieveFromYt = response.Items.SingleOrDefault();

            if (userRetrieveFromYt == null)
            {
                throw new BadLinkException("This user doesn't exist");
            }
            else
            {
                if (userRetrieveFromYt.Statistics.HiddenSubscriberCount == true)
                {
                    throw new BadLinkException("You must not hide the subscribers counter");
                }

                var currentUserId = User.Identity.GetUserId();
                newYoutubePage.UserId = currentUserId;
                newYoutubePage.YoutubeUsername = userRetrieveFromYt.Id;
                var ytAlreadyAdded = _context.Youtubes.SingleOrDefault(y => y.YoutubeUsername == newYoutubePage.YoutubeUsername);


                if (ytAlreadyAdded == null)
                {
                    _context.Youtubes.Add(newYoutubePage);
                }
                else
                {
                    if (ytAlreadyAdded.UserId.Equals(currentUserId))
                    {
                        ytAlreadyAdded.Deleted = false;

                    }
                    else
                    {
                        throw new LinkAlreadyExistException("This channel is already added on this site");
                    }
                }

            }
        }

        public ActionResult Add()
        {
            var listaSocialNetworks = _context.SocialNetworks.ToList();
            var model = new SocialPageFormViewModel()
            {
                SocialNetworks = listaSocialNetworks,
                Cpc = 10
            };
            return View("Add", model);
        }

        [Route("SocialPage/Edit/{type:regex(facebook|twitter|youtube)}/{id}")]
        public ActionResult Edit(string type, int id)
        {
            var currentUserId = User.Identity.GetUserId();
            SocialPage page = null;
            var SocialPageEdit = new EditViewModel();


            if (type.Equals("facebook"))
            {
                page = _context.Facebooks.SingleOrDefault(fb => fb.Id == id);
                SocialPageEdit.SocialNetworkId = SocialNetwork.FacebookPageLikes;
            }
            else if (type.Equals("twitter"))
            {
                page = _context.Twitters.SingleOrDefault(twt => twt.Id == id);
                SocialPageEdit.SocialNetworkId = SocialNetwork.TwitterFollowers;
            }
            else if (type.Equals("youtube"))
            {
                page = _context.Youtubes.SingleOrDefault(yt => yt.Id == id);
                SocialPageEdit.SocialNetworkId = SocialNetwork.YoutubeSubscribers;
            }
            else
            {
                return HttpNotFound();
            }
            if (page == null)
            {
                return HttpNotFound();
            }


            SocialPageEdit.Id = page.Id;
            SocialPageEdit.Name = page.Name;
            SocialPageEdit.Cpc = page.Cpc;
            SocialPageEdit.Active = page.Active;

            return View(SocialPageEdit);
        }

        [HttpPost]
        public ActionResult Save(EditViewModel editSocial)
        {
            if (!ModelState.IsValid)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            SocialPage socialPage = null;
            if (editSocial.SocialNetworkId == 1)
            {
                socialPage = _context.Facebooks.SingleOrDefault(fb => fb.Id == editSocial.Id && fb.UserId == userId);
            }
            else if (editSocial.SocialNetworkId == 2)
            {
                socialPage = _context.Twitters.SingleOrDefault(twt => twt.Id == editSocial.Id && twt.UserId == userId);
            }
            else if (editSocial.SocialNetworkId == 3)
            {
                socialPage = _context.Youtubes.SingleOrDefault(yt => yt.Id == editSocial.Id && yt.UserId == userId);
            }
            else
            {
                return HttpNotFound();
            }

            if (socialPage == null)
            {
                return HttpNotFound();
            }
            socialPage.Active = editSocial.Active;
            socialPage.Cpc = editSocial.Cpc;
            socialPage.Name = editSocial.Name;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var listaFacebook = _context.Facebooks.Where(fb => fb.UserId.Equals(userId) && fb.Deleted == false).ToList();
            var listaTwitter = _context.Twitters.Where(twt => twt.UserId.Equals(userId) && twt.Deleted == false).ToList();
            var listaYoutube = _context.Youtubes.Where(yt => yt.UserId.Equals(userId) && yt.Deleted == false).ToList();

            var mySitesViewModel = new MySocialPagesViewModel()
            {
                FacebookPages = listaFacebook,
                TwitterPages = listaTwitter,
                YoutubePages = listaYoutube,
                numberOfPages = listaFacebook.Count + listaTwitter.Count + listaYoutube.Count
            };


            return View(mySitesViewModel);
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            _twitterContext.Dispose();
        }
    }
}