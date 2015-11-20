﻿using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebMVC.Models.TrackersViewModels;
using Microsoft.AspNet.Identity;

namespace WebMVC.Controllers
{
    [Authorize]
    public class PersonalAreaController : BaseController
    {
        // GET: PersonalArea
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();

            // Профайл текущего пользователя
            UserProfile model = dbContext.UserProfiles.FirstOrDefault(x => x.User.Id == currentUserId);

            // Если в личный кабинет пользователь заходит первый раз и профайла у него еще нет, то его надо создать
            if (model == null)
            {
                model = new UserProfile()
                {
                    User = dbContext.Users.FirstOrDefault(x => x.Id == currentUserId)
                };

                dbContext.UserProfiles.Add(model);
                dbContext.SaveChanges();
            }

            return View(model);
        }

        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currenUserId = User.Identity.GetUserId();

            GPSTracker tracker = new GPSTracker()
            {
                Id = model.Id,
                Owner = dbContext.Users.Find(currenUserId).UserProfile,
                PhoneNumber = model.PhoneNumber,
                Name = model.Name
            };
            
            dbContext.GPSTrackers.Add(tracker);
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GPSTracker tracker = dbContext.GPSTrackers.Find(id);

            if (tracker == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (tracker.Owner.User.Id != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            EditViewModel model = new EditViewModel()
            {
                Id = tracker.Id,
                PhoneNumber = tracker.PhoneNumber,
                Name = tracker.Name
            };            

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            GPSTracker tracker = dbContext.GPSTrackers.Find(model.Id);

            if (tracker == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (tracker.Owner.User.Id != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            tracker.PhoneNumber = model.PhoneNumber;
            tracker.Name = model.Name;

            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Map()
        {
            return View();
        }

        public ActionResult ConfirmDelete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GPSTracker tracker = dbContext.GPSTrackers.Find(id);

            if (tracker == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (tracker.Owner.User.Id != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            return View(tracker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GPSTracker tracker = dbContext.GPSTrackers.Find(id);

            if (tracker == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (tracker.Owner.User.Id != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            dbContext.GPSTrackers.Remove(tracker);
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}