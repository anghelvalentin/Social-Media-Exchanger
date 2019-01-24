using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Social_Media_Exchanger.Models;
using LinqToTwitter;
using System.Net;

namespace Social_Media_Exchanger.Controllers
{
    [Authorize]
    public class TwitterController : Controller
    {
        ApplicationDbContext _context;
        TwitterContext _twitterContext;

        public TwitterController()
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

        public ActionResult getData(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpNotFound();
            }

            var twitterUser = _context.Twitters.Where(twt => twt.Id == id).SingleOrDefault();

            if (twitterUser == null)
            {
                return HttpNotFound();
            }


            var userFromTwitter = _twitterContext.User.Where(twt => twt.Type == UserType.Show && twt.ScreenName == twitterUser.TwitterUsername).SingleOrDefault();
            if (userFromTwitter == null)
            {
                return HttpNotFound();
            }

            TempData["numberOfFollowers"] = userFromTwitter.FollowersCount;

            return Redirect(twitterUser.Url);
        }


        [HttpPost]
        public ActionResult GetFollowersAfter(int id)
        {
            if (!ModelState.IsValid && !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index");
            }

            var twitterPage = _context.Twitters.Where(twt => twt.Id == id).SingleOrDefault();
            if (twitterPage == null)
            {
                return HttpNotFound();
            }

            var twitterUser = _twitterContext.User
                .Where(u => u.Type == UserType.Show && u.ScreenName == twitterPage.TwitterUsername)
                .SingleOrDefault();

            if (twitterUser == null)
            {
                ViewBag.text = "Please skip this page";
                return PartialView("Message");
            }

            var numberOfFOllowers = twitterUser.FollowersCount;

            if (numberOfFOllowers > Convert.ToInt32(TempData["numberOfFollowers"]))
            {
                var idUser = User.Identity.GetUserId();
                var userWhoLiked = _context.Users.Where(u => u.Id == idUser).Single();

                if (userWhoLiked != null)
                {
                    userWhoLiked.Points = userWhoLiked.Points + twitterPage.Cpc - 1;
                    twitterPage.User.Points = twitterPage.User.Points - twitterPage.Cpc;
                    twitterPage.NumberOfClicks = twitterPage.NumberOfClicks + 1;
                    var twitterFollow = new TwitterFollowers() { TwitterPageId = twitterPage.Id, UserId = userWhoLiked.Id };
                    userWhoLiked.ClicksToday += 1;
                    _context.Follows.Add(twitterFollow);
                    _context.SaveChanges();
                    ViewBag.text = "You succesfully like " + twitterPage.Name;
                    ViewBag.points = userWhoLiked.Points;
                    ViewBag.pageid = twitterPage.Id;
                }
                return PartialView("_TextConfirmation", twitterPage);
            }
            else
            {
                TempData["numberOfFollowers"] = "9999999";
                ViewBag.text = "You didn't follow this page.";
                return PartialView("Message");
            }
        }

        public ActionResult Edit()
        {
            var currentUserId = User.Identity.GetUserId();
            var socialPage = _context.Facebooks.Where(fb => fb.UserId == currentUserId).FirstOrDefault();

            return View(socialPage);
        }

        [HttpPost]
        public ActionResult Edit(SocialPage socialPage)
        {
            if (!ModelState.IsValid)
            {
                return HttpNotFound();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Skip(int id)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var twtFollow = new TwitterFollowers()
                {
                    TwitterPageId = id,
                    UserId = userId
                };
                _context.Follows.Add(twtFollow);
                _context.SaveChanges();
            }
            ViewBag.text = "You skipped";
            return PartialView("Message");
        }


        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null || !Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var twtPage = _context.Twitters.Where(twt => twt.Id == id).SingleOrDefault();

            if (twtPage == null)
            {
                return HttpNotFound();
            }

            twtPage.Deleted = true;
            _context.SaveChanges();

            ViewBag.pageId = id;
            ViewBag.text = "You succesfully deleted " + twtPage.Name;


            return PartialView("_PageDeleted");
        }



        //
        // GET: /Twitter/
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            var PageIds = _context.Follows.Where(t => t.UserId == userID).Select(t => t.TwitterPageId).ToList();
            var items = _context.Twitters.Where(t => t.UserId != userID && t.Deleted == false && t.User.Points > 0 && t.Active == true && !PageIds.Contains(t.Id))
                .OrderByDescending(fb => fb.Cpc)
                .Take(10);
            return View(items);
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            _twitterContext.Dispose();
        }
    }
}