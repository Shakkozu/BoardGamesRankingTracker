﻿using BoardGamesRankingTracker.Models;
using Dapper;
using Microsoft.AspNet.Identity;
using RankingTrackerLibrary.Data;
using RankingTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BoardGamesRankingTracker.Controllers
{
    public class PlayerController : Controller
    {
        // GET: Player
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                Player player = new Player();
                using (IDbConnection cnn = new SqlConnection(GlobalConfig.CnnString()))
                {
                    Player result = cnn.Query<Player>($"select * From Players Where Id={id}").First();
                    PlayerViewModel viewModel = new PlayerViewModel { EmailAddress = result.EmailAddress, Nickname = result.Nickname };
                    if (result != null)
                        return View(viewModel);
                }
                return RedirectToAction("Search");
            }
            else
            {
                var userId = User.Identity.GetUserId();
                if(userId != null)
                {
                Player result =  GlobalConfig.Connection.GetPlayer_ByOwnerId(userId);
                PlayerViewModel viewModel = new PlayerViewModel { EmailAddress = result.EmailAddress, Nickname = result.Nickname, JoinedOn = result.Joined };
                    if (result != null)
                        return View(viewModel);
                }
            

            }
            return RedirectToAction("Index", "Home");
        }


        //GET : Search
        public ActionResult Search()
        {
            return View();
        }
    }
}