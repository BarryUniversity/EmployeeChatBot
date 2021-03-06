﻿using EmployeeChatBot.Data.Access.Abstraction;
using EmployeeChatBot.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EmployeeChatBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly IReportAccess _reportAccess;

        public HomeController(IReportAccess reportAccess)
        {
            _reportAccess = reportAccess;
        }

        public async Task<IActionResult> SaveReport(ReportModel model)
        {
            if (model.Symptoms == null)
                model.Symptoms = "";

            if (model.ReportId == null)
                return Forbid();

            int reportId = Convert.ToInt32(model.ReportId);
            var reportCheck = await _reportAccess.CheckReport(reportId);
            if (reportCheck.CompletedAt != null)
            {
                return Forbid();
            }

            var coughing = model.Symptoms.Contains("Cough");
            var fever = model.Symptoms.Contains("Temperature");
            var breathing = model.Symptoms.Contains("Breathing");
            var soreThroat = model.Symptoms.Contains("Sore Throat");
            var bodyAches = model.Symptoms.Contains("Body Aches");
            var lossOfSmell = model.Symptoms.Contains("Loss of taste or smell");
            await _reportAccess.SaveReport(reportId, fever,
                coughing, breathing, soreThroat,
                bodyAches, false, lossOfSmell);

            return Ok();
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModel model)
        {
            //Login Logic here

            string email = string.Empty;
            string user = string.Empty;
            string name = string.Empty;
            int id = 0;
#if DEBUG
            //Remove when login logic is available
            email = "";
            user = "";
            name = "";
            id = 0;
#endif

            ReportDataModel newReport = await _reportAccess.CreateReport(user, id, email);

            IndexViewModel indexViewModel = new IndexViewModel
            {
                ReportId = newReport.Id,
                UserAdEmail = email,
                UserAdName = name,
            };

            return RedirectToAction("Index", indexViewModel);
        }

        [HttpGet]
        public IActionResult Index(IndexViewModel model)
        {
            var dayOfWeek = DateTime.Now.DayOfWeek.ToString();
            var chatBotImage = "CB1A.PNG";
            switch (dayOfWeek)
            {
                case "Monday":
                    chatBotImage = "CB2A.PNG";
                    break;
                case "Tuesday":
                    chatBotImage = "CB3A.PNG";
                    break;
                case "Wednesday":
                    chatBotImage = "CB4A.PNG";
                    break;
                case "Thursday":
                    chatBotImage = "CB5A.PNG";
                    break;
                case "Friday":
                    chatBotImage = "CB6A.PNG";
                    break;
                case "Saturday":
                    chatBotImage = "CB7A.PNG";
                    break;
                case "Sunday":
                    chatBotImage = "CB1A.PNG";
                    break;
            }
            model.BotImage = chatBotImage;

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}