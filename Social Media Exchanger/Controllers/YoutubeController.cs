using Social_Media_Exchanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.Net;

namespace Social_Media_Exchanger.Controllers
{
    [Authorize]
    public class YoutubeController : Controller
    {
        ApplicationDbContext _context;
        YouTubeService _youtubeService;

        public YoutubeController()
        {
            _context = new ApplicationDbContext();
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDp5rhMI0dW7uHIYhq0_iLS9vW5pgy1oy0",
            });
        }


        public ActionResult GetData(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpNotFound();
            }


            var ytChannel = _context.Youtubes.Where(yt => yt.Id == id).SingleOrDefault();

            if (ytChannel == null)
            {
                return HttpNotFound();
            }

            try
            {
                var request = _youtubeService.Channels.List("statistics");

                request.Id = ytChannel.YoutubeUsername;


                var channel = request.Execute().Items[0];

                var numberOfLikes = channel.Statistics.SubscriberCount;
                if (numberOfLikes == null)
                {
                    return Content("Please skip this page");
                }

                TempData["numberOfSubscribers"] = numberOfLikes;
                return Redirect(ytChannel.Url);
            }
            catch (Exception)
            {
                ViewBag.text = "This page is broken. Please skip it.";
                return PartialView("Message");
            }
        }

        [HttpPost]
        public ActionResult GetSubscribersAfter(int id)
        {
            if (!ModelState.IsValid && !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index");
            }

            var youtubePage = _context.Youtubes.Where(yt => yt.Id == id).SingleOrDefault();
            if (youtubePage == null)
            {
                return HttpNotFound();
            }

            var request = _youtubeService.Channels.List("statistics");
            request.Id = youtubePage.YoutubeUsername;


            var channel = request.Execute().Items[0];

            var numberOfLikes = channel.Statistics.SubscriberCount;

            if (numberOfLikes == null)
            {
                ViewBag.text = "This page is broken. Please skip it.";
                return PartialView("Message");
            }


            if (numberOfLikes > UInt64.Parse(TempData["numberOfSubscribers"].ToString()))
            {
                var idUser = User.Identity.GetUserId();
                var userWhoLiked = _context.Users.Where(u => u.Id == idUser).Single();

                if (userWhoLiked != null)
                {
                    userWhoLiked.Points = userWhoLiked.Points + youtubePage.Cpc - 1;
                    youtubePage.User.Points = youtubePage.User.Points - youtubePage.Cpc;
                    youtubePage.NumberOfClicks = youtubePage.NumberOfClicks + 1;
                    var ytSubscribe = new YoutubeSubscribtion() { YoutubePageId = youtubePage.Id, UserId = userWhoLiked.Id };
                    userWhoLiked.ClicksToday += 1;
                    _context.YtSubscribes.Add(ytSubscribe);
                    _context.SaveChanges();
                    ViewBag.text = "You succesfully like " + youtubePage.Name;
                    ViewBag.points = userWhoLiked.Points;
                    ViewBag.pageid = youtubePage.Id;
                }
                return PartialView("_TextConfirmation", youtubePage);
            }
            else
            {
                TempData["numberOfLikes"] = "9999999";
                ViewBag.text = "You didn't subscribe this channel.";
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
        public ActionResult Skip(int id)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var ytSubscribe = new YoutubeSubscribtion()
                {
                    YoutubePageId = id,
                    UserId = userId
                };
                _context.YtSubscribes.Add(ytSubscribe);
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

            var ytPage = _context.Youtubes.Where(yt => yt.Id == id).SingleOrDefault();

            if (ytPage == null)
            {
                return HttpNotFound();
            }

            ytPage.Deleted = true;
            _context.SaveChanges();

            ViewBag.pageId = id;
            ViewBag.text = "You succesfully deleted " + ytPage.Name;


            return PartialView("_PageDeleted");
        }



        //
        // GET: /Youtube/
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            var PageIds = _context.YtSubscribes.Where(y => y.UserId == userID).Select(y => y.YoutubePageId).ToList();
            var items = _context.Youtubes.Where(yt =>
                yt.UserId != userID
                && yt.Deleted == false
                && yt.User.Points > 0
                && yt.Active == true
                && !PageIds.Contains(yt.Id))
                .OrderByDescending(fb => fb.Cpc)
                .Take(10);
            return View(items);
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
    }
}