using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using Social_Media_Exchanger.Models;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Social_Media_Exchanger.ViewModels;
using Microsoft.AspNet.Identity;
using System.Diagnostics;
using Social_Media_Exchanger.Utils;


namespace Social_Media_Exchanger.Controllers
{
    [Authorize]
    public class FacebookController : Controller
    {
        ApplicationDbContext _context;

        public FacebookController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult GetData(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpNotFound();
            }


            var url = _context.Facebooks.Where(fb => fb.Id == id).Select(fb => fb.Url).SingleOrDefault();

            if (String.IsNullOrWhiteSpace(url))
            {
                return HttpNotFound();
            }

            try
            {
                FacebookGraph fbGraph = new FacebookGraph(url);
                string numberOfLikes = fbGraph.getNumberOfLikes();
                if (String.IsNullOrWhiteSpace(numberOfLikes))
                {
                    ViewBag.text = "Please skip this page";
                    return PartialView("Message");
                }

                TempData["numberOfLikes"] = numberOfLikes;
                return Redirect(url);
            }
            catch (Exception)
            {
                ViewBag.text = "This page is broken. Please skip it.";
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
            //var fbPage= _context.Facebooks.Where(fb=>fb.FacebookId == id).SingleOrDefault();

            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var fbLiked = new FacebookLiked()
                {
                    FacebookPageId = id,
                    UserId = userId
                };
                _context.FacebookLikes.Add(fbLiked);
                _context.SaveChanges();
            }
            ViewBag.text = "You skipped";
            return PartialView("Message");
        }


        [HttpPost]
        public ActionResult GetLikesAfter(int id)
        {
            if (!ModelState.IsValid && !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index");
            }

            var facebookPage = _context.Facebooks.Where(fb => fb.Id == id).SingleOrDefault();
            if (facebookPage == null)
            {
                return HttpNotFound();
            }

            FacebookGraph fbGraph = new FacebookGraph(facebookPage.Url);
            var numberOfLikes = fbGraph.getNumberOfLikes();

            if (numberOfLikes == null)
            {
                ViewBag.text = "Broken Page. Please skip";
                return PartialView("Message");
            }

            if (Convert.ToInt32(numberOfLikes) > Convert.ToInt32(TempData["numberOfLikes"]))
            {
                var idUser = User.Identity.GetUserId();
                var userWhoLiked = _context.Users.Where(u => u.Id == idUser).Single();

                if (userWhoLiked != null)
                {
                    userWhoLiked.Points = userWhoLiked.Points + facebookPage.Cpc - 1;
                    facebookPage.User.Points = facebookPage.User.Points - facebookPage.Cpc;
                    facebookPage.NumberOfClicks = facebookPage.NumberOfClicks + 1;
                    userWhoLiked.ClicksToday += 1;
                    var facebookLike = new FacebookLiked() { FacebookPageId = facebookPage.Id, UserId = userWhoLiked.Id };
                    _context.FacebookLikes.Add(facebookLike);
                    _context.SaveChanges();
                    ViewBag.text = "You succesfully like " + facebookPage.Name;
                    ViewBag.points = userWhoLiked.Points;
                    ViewBag.pageid = facebookPage.Id;
                }
                return PartialView("_TextConfirmation", facebookPage);
            }
            else
            {
                TempData["numberOfLikes"] = "9999999";
                ViewBag.text = "You didn't like this page.";
                return PartialView("Message");

            }
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null || !Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var fbPage = _context.Facebooks.Where(fb => fb.Id == id).SingleOrDefault();

            if (fbPage == null)
            {
                return HttpNotFound();
            }

            fbPage.Deleted = true;
            _context.SaveChanges();

            ViewBag.pageId = id;
            ViewBag.text = "You succesfully deleted " + fbPage.Name;


            return PartialView("_PageDeleted");
        }


        //
        // GET: /Facebook/
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            var PageIds = _context.FacebookLikes.Where(f => f.UserId == userID).Select(f => f.FacebookPageId).ToList();
            var items = _context.Facebooks.Where(fb => fb.UserId != userID && fb.Deleted == false && fb.User.Points > 0 && fb.Active == true && !PageIds.Contains(fb.Id))
                .OrderByDescending(fb => fb.Cpc)
                .Take(10);
            return View(items);
        }


    }
}