using System;
using System.Linq;
using DutchTreat.Data;
using DutchTreat.Services;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DutchTreat.Controllers
{
    public class AppController : Controller
    {
        private readonly IMailService _mailService;
        public readonly DutchContext _ctx;

        public AppController(IMailService mailService, DutchContext ctx)
        {
            _mailService = mailService;
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            var results = _ctx.Products.ToList();
            return View(results);
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("contact")]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                _mailService.SendMessage(
                    "sergiigrytsaienko@gmail.com",
                    model.Subject,
                    $"From: {model.Name} - {model.Email}, Message: {model.Message}"
                );
                ViewBag.UserMessage = "Mail Sent";
                ModelState.Clear();
            }

            return View();
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            ViewBag.Title = "About Us";
            return View();
        }

        public IActionResult Shop()
        {
            ViewBag.Title = "About Us";
            var results = _ctx.Products
                .OrderBy(p => p.Category)
                .ToList();

//            2nd option            
//            var results = from p in _ctx.Products
//                orderby p.Category
//                select p;

            return View(results.ToList());
        }
    }
}