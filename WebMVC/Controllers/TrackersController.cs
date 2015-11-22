﻿using DAL.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebMVC.Models.TrackersViewModels;

namespace WebMVC.Controllers
{
    [Authorize]
    public class TrackersController : BaseController
    {
        
        public ActionResult Index(GPSTracker tracker)
        {
            string currentUserId = User.Identity.GetUserId();
            UserProfile model = dbContext.UserProfiles.FirstOrDefault(x => x.User.Id == currentUserId);

            ICollection<GPSTrackerMessage> messages = new List<GPSTrackerMessage>();

            messages = dbContext.GPSTrackerMessages.Where(x => x.GPSTrackerId == tracker.Id).ToList();
            //ICollection<GPSTrackerMessage> trackerMessages = new List<GPSTrackerMessage>();

            //foreach (GPSTracker tracker in trackers)
            //{
            //    trackerMessages.Add(tracker.GPSTrackerMessages.Last());
            //}

            //return View(trackerMessages);

            return View(messages);
        }

    }
}